/*
 * Copyright (C) 2019 Consiglio Regionale della Lombardia
 * SPDX-License-Identifier: AGPL-3.0-or-later
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PortaleRegione.API.Helpers;
using PortaleRegione.API.it.lombardia.regione.consiglio.intranet;
using PortaleRegione.BAL;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Autenticazione;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Domain.Essentials;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;
using PortaleRegione.DTO.Response;
using PortaleRegione.Logger;

namespace PortaleRegione.API.Controllers
{
    /// <summary>
    ///     Controller per l'autenticazione
    /// </summary>
    [AllowAnonymous]
    public class AutenticazioneController : BaseApiController
    {
        private readonly PersoneLogic _logicPersona;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     ctor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="logicPersona"></param>
        public AutenticazioneController(IUnitOfWork unitOfWork, PersoneLogic logicPersona)
        {
            _unitOfWork = unitOfWork;
            _logicPersona = logicPersona;
        }

        /// <summary>
        ///     Endpoint di login
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginRequest loginModel)
        {
            try
            {
                if (string.IsNullOrEmpty(loginModel.Username)) return BadRequest("Username non può essere nullo");

                if (string.IsNullOrEmpty(loginModel.Password))
                    return BadRequest("Password non può essere nulla");

                if (loginModel.Username == AppSettingsConfiguration.Service_Username &&
                    loginModel.Password == AppSettingsConfiguration.Service_Password)
                {
                    var tokenService = GetToken(new PersonaDto
                    {
                        CurrentRole = RuoliIntEnum.SERVIZIO_JOB,
                        cognome = "SERVIZIO",
                        nome = "JOB"
                    });

                    return Ok(new LoginResponse
                    {
                        jwt = tokenService
                    });
                }

                var intranetAdService = new proxyAD();

#if DEBUG == true
                if (loginModel.Username.Contains("***"))
                {
                    //HACK LOGIN
                    Log.Debug("### LOGIN HACKERATO ###");
                    loginModel.Username = loginModel.Username.Replace("***", "");
                }
                else
                {
#endif

                    //var passwordExpire = intranetAdService.PasswordExpire(loginModel.Username, loginModel.Password,
                    //    "CONSIGLIO",
                    //    AppSettingsConfiguration.TOKEN_R);
                    //Console.WriteLine($"Ingresso --> la password dell'utente scadrà tra {passwordExpire} giorni");

                    var authResult = intranetAdService.Authenticate(
                        loginModel.Username,
                        loginModel.Password,
                        "CONSIGLIO",
                        AppSettingsConfiguration.TOKEN_R);
                    //var authResult = true;
                    if (!authResult)
                        return BadRequest(
                            "Nome Utente o Password non validi! Utilizza le credenziali di accesso al pc ([nome.cognome] - [propriapassword])");
#if DEBUG == true
                }
#endif
                var persona = _unitOfWork.Persone.Get(@"CONSIGLIO\" + loginModel.Username);
                if (persona == null)
                    return BadRequest(
                        "Autenticazione corretta, ma l'utente non risulta presente nel sistema. Contattare l'amministratore di sistema.");

                var personaDto = Mapper.Map<View_UTENTI, PersonaDto>(persona);

                var lRuoli = new List<string>();
                var Gruppi_Utente = new List<string>(intranetAdService.GetGroups(
                    loginModel.Username.Replace(@"CONSIGLIO\", ""), "PEM_", AppSettingsConfiguration.TOKEN_R));

                if (Gruppi_Utente.Count == 0)
                    return BadRequest("Utente non configurato correttamente.");

                foreach (var group in Gruppi_Utente)
                    lRuoli.Add($"CONSIGLIO\\{group}");

                personaDto.Carica = _logicPersona.GetCaricaPersona(personaDto.UID_persona);

                var token = GetToken(personaDto, lRuoli);

                return Ok(new LoginResponse
                {
                    jwt = token,
                    id = personaDto.UID_persona,
                    persona = personaDto
                });
            }
            catch (Exception e)
            {
                Log.Error("Login", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per il cambio ruolo
        /// </summary>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetToken(RuoliIntEnum ruolo)
        {
            try
            {
                var ruoloInDb = _unitOfWork.Ruoli.Get((int) ruolo);
                if (ruoloInDb == null)
                    return BadRequest("Ruolo non trovato");

                var intranetAdService = new proxyAD();
                var Gruppi_Utente = new List<string>(intranetAdService.GetGroups(
                    currentUser.Persona.userAD.Replace(@"CONSIGLIO\", ""), "PEM_", AppSettingsConfiguration.TOKEN_R));

                var lRuoli = Gruppi_Utente.Select(group => $"CONSIGLIO\\{group}").ToList();

                var ruoli_utente = _unitOfWork.Ruoli.RuoliUtente(lRuoli).ToList();

                var ruoloAccessibile = ruoli_utente.SingleOrDefault(r => r.IDruolo == (int) ruolo);
                if (ruoloAccessibile == null)
                    return BadRequest("Ruolo non accessibile");

                var persona = currentUser.Persona;
                persona.CurrentRole = ruolo;
                persona.Gruppo = _unitOfWork.Gruppi.GetGruppoPersona(lRuoli, persona.IsGiunta());
                persona.Carica = _logicPersona.GetCaricaPersona(persona.UID_persona);

                var token = GetToken(persona);

                return Ok(new LoginResponse
                {
                    jwt = token,
                    id = persona.UID_persona,
                    persona = persona
                });
            }
            catch (Exception e)
            {
                Log.Error("GetToken - Ruoli", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per il cambio gruppo
        /// </summary>
        /// <param name="gruppo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IHttpActionResult GetToken(int gruppo)
        {
            try
            {
                var gruppoInDb = _unitOfWork.Gruppi.Get(gruppo);
                if (gruppoInDb == null)
                    return BadRequest("ListaGruppo non trovato");

                var gruppoDto = Mapper.Map<gruppi_politici, GruppiDto>(gruppoInDb);
                var persona = currentUser.Persona;
                persona.Gruppo = gruppoDto;
                persona.CurrentRole = RuoliIntEnum.Responsabile_Segreteria_Politica;
                var token = GetToken(persona);
                
                return Ok(new LoginResponse
                {
                    jwt = token,
                    id = persona.UID_persona,
                    persona = persona
                });
            }
            catch (Exception e)
            {
                Log.Error("GetToken", e);
                return ErrorHandler(e);
            }
        }

        #region GetToken

        /// <summary>
        ///     Metodo per calcolare il JWT Token
        /// </summary>
        /// <param name="personaDto"></param>
        /// <param name="lRuoli_Gruppi"></param>
        /// <returns></returns>
        private string GetToken(PersonaDto personaDto, List<string> lRuoli_Gruppi)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(
                    AppSettingsConfiguration.JWT_MASTER);

                var ruoli_utente = _unitOfWork.Ruoli.RuoliUtente(lRuoli_Gruppi).ToList();
                personaDto.Ruoli = ruoli_utente.Select(Mapper.Map<RUOLI, RuoliDto>);
                personaDto.CurrentRole = (RuoliIntEnum) ruoli_utente[0].IDruolo;
                personaDto.Gruppo = _unitOfWork.Gruppi.GetGruppoPersona(lRuoli_Gruppi, personaDto.IsGiunta());

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, ruoli_utente[0].IDruolo.ToString()),
                    new Claim(ClaimTypes.Name, personaDto.UID_persona.ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = AppSettingsConfiguration.URL_API,
                    Issuer = AppSettingsConfiguration.URL_API,
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(AppSettingsConfiguration.JWT_EXPIRATION),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                Log.Error("GetToken", e);
                throw e;
            }
        }

        /// <summary>
        ///     Metodo per calcolare il JWT Token
        /// </summary>
        /// <param name="personaDto"></param>
        /// <returns></returns>
        private string GetToken(PersonaDto personaDto)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(
                    AppSettingsConfiguration.JWT_MASTER);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, Convert.ToInt32(personaDto.CurrentRole).ToString()),
                    new Claim(ClaimTypes.Name, personaDto.UID_persona.ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = AppSettingsConfiguration.URL_API,
                    Issuer = AppSettingsConfiguration.URL_API,
                    Subject = new ClaimsIdentity(claims),
                    NotBefore = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddMinutes(AppSettingsConfiguration.JWT_EXPIRATION),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                Log.Error("GetToken", e);
                throw e;
            }
        }

        #endregion
    }
}