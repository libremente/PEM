﻿/*
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
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using PortaleRegione.API.Helpers;
using PortaleRegione.API.it.lombardia.regione.consiglio.intranet;
using PortaleRegione.BAL;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;
using PortaleRegione.Logger;

namespace PortaleRegione.API.Controllers
{
    /// <summary>
    ///     Controller per gestire le persone
    /// </summary>
    [Authorize]
    [RoutePrefix("persone")]
    public class PersoneController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersoneController(PersoneLogic logicPersone, IUnitOfWork unitOfWork) : base(logicPersone)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     Endpoint per avere le informazioni di una persona
        /// </summary>
        /// <param name="id"></param>
        /// <param name="IsGiunta"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> GetPersona(Guid id, bool IsGiunta = false)
        {
            try
            {
                return Ok(await _logicPersone.GetPersona(id, IsGiunta));
            }
            catch (Exception e)
            {
                Log.Error("GetPersona", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere informazioni sul ruolo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ruolo/{id:int}")]
        public async Task<IHttpActionResult> GetRuolo(int id)
        {
            try
            {
                var ruolo = await _unitOfWork.Ruoli.Get(id);
                return Ok(Mapper.Map<RUOLI, RuoliDto>(ruolo));
            }
            catch (Exception e)
            {
                Log.Error("GetRuolo", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere le informazioni sul capo gruppo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("gruppo/{id:int}/capo-gruppo")]
        public async Task<IHttpActionResult> GetCapoGruppo(int id)
        {
            try
            {
                var capoGruppo = await _unitOfWork.Gruppi.GetCapoGruppo(id);
                return Ok(Mapper.Map<View_UTENTI, PersonaDto>(capoGruppo));
            }
            catch (Exception e)
            {
                Log.Error("GetCapoGruppo", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere le persone che compongono la segreteria del gruppo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notifica_firma"></param>
        /// <param name="notifica_deposito"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("gruppo/{id:int}/segreteria-politica")]
        public async Task<IHttpActionResult> GetSegreteriaPolitica(int id, bool notifica_firma, bool notifica_deposito)
        {
            try
            {
                var segreteriaPolitica =
                    await _unitOfWork.Gruppi.GetSegreteriaPolitica(id, notifica_firma, notifica_deposito);
                return Ok(segreteriaPolitica.Select(Mapper.Map<UTENTI_NoCons, PersonaDto>));
            }
            catch (Exception e)
            {
                Log.Error("GetSegreteriaPolitica", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere le persone che compongono la giunta regionale
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("giunta-regionale")]
        public async Task<IHttpActionResult> GetGiuntaRegionale()
        {
            try
            {
                var giuntaRegionale = await _unitOfWork.Persone.GetGiuntaRegionale();
                return Ok(giuntaRegionale.Select(Mapper.Map<View_Composizione_GiuntaRegionale, PersonaDto>));
            }
            catch (Exception e)
            {
                Log.Error("GetGiuntaRegionale", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere tutti gli assessori di riferimento per la legislatura attuale
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("assessori")]
        public async Task<IHttpActionResult> GetAssessoriRiferimento()
        {
            try
            {
                var personaDtos = (await _unitOfWork.Persone
                    .GetAssessoriRiferimento(await _unitOfWork.Legislature.Legislatura_Attiva()))
                    .Select(Mapper.Map<View_UTENTI, PersonaDto>);

                return Ok(personaDtos);
            }
            catch (Exception e)
            {
                Log.Error("GetAssessoriRiferimento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere tutti i relatori per la legislatura attuale
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("relatori")]
        public async Task<IHttpActionResult> GetRelatori(Guid? id)
        {
            try
            {
                return Ok(await _logicPersone.GetRelatori(id));
            }
            catch (Exception e)
            {
                Log.Error("GetRelatori", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere tutti i gruppi per la legislatura attuale
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("gruppi")]
        public async Task<IHttpActionResult> GetGruppi()
        {
            try
            {
                return Ok(await _logicPersone.GetGruppi());
            }
            catch (Exception e)
            {
                Log.Error("GetGruppi", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint richiesta cambio pin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cambio-pin")]
        public async Task<IHttpActionResult> CambioPin(CambioPinModel model)
        {
            try
            {
                if (model.conferma_pin != model.nuovo_pin)
                    return BadRequest("Il nuovo PIN non combacia con quello di conferma!!!");

                var persona = await GetSession();
                var currentPin = await _logicPersone.GetPin(persona);
                if (currentPin == null)
                    return BadRequest("Pin non impostato");
                if (currentPin.RichiediModificaPIN)
                    return BadRequest("E' richiesto il reset del pin");
                if (currentPin.PIN_Decrypt != model.vecchio_pin)
                    return BadRequest("Il vecchio PIN non è corretto!!!");

                int valuePin;
                var checkTry = int.TryParse(model.nuovo_pin, out valuePin);
                if (!checkTry)
                    return BadRequest("Il pin deve contenere solo cifre numeriche");
                if (model.nuovo_pin.Length != 4)
                    return BadRequest("Il PIN dev'essere un numero di massimo 4 cifre!");

                model.PersonaUId = persona.UID_persona;

                await _logicPersone.CambioPin(model);

                return Ok("OK");
            }
            catch (Exception e)
            {
                Log.Error("CambioPin", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere tutti le persone attive dalla vista utenti
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpGet]
        public async Task<IHttpActionResult> GetPersone()
        {
            try
            {
                var persone = (await _unitOfWork
                    .Persone
                    .GetAll_DA_CANCELLARE())
                    .Select(Mapper.Map<View_UTENTI, PersonaDto>);

                return Ok(persone);
            }
            catch (Exception e)
            {
                Log.Error("GetPersone", e);
                return ErrorHandler(e);
            }
        }

        [Authorize(Roles = RuoliEnum.Amministratore_PEM)]
        [HttpPost]
        [Route("reset-pwd")]
        public async Task<IHttpActionResult> ResetPassword(string nuova)
        {
            try
            {
                string ris;
                var IntranetADService = new proxyAD();
                ris = IntranetADService
                    .ChangeADUserPass(
                        (await GetSession()).userAD.Replace(@"CONSIGLIO\", ""),
                        string.Empty,
                        nuova,
                        AppSettingsConfiguration.TOKEN_W);
                if (!ris.Contains("0:")) return BadRequest("Reset password non riuscito. Motivo: " + ris);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("ResetPassword", e);
                return ErrorHandler(e);
            }
        }

        [Authorize(Roles = RuoliEnum.Amministratore_PEM)]
        [HttpPost]
        [Route("reset-pin")]
        public async Task<IHttpActionResult> ResetPin(ResetPinModel model)
        {
            try
            {
                int valuePin;
                var checkTry = int.TryParse(model.nuovo_pin, out valuePin);
                if (!checkTry)
                    return BadRequest("Il pin deve contenere solo cifre numeriche");
                if (model.nuovo_pin.Length != 4)
                    return BadRequest("Il PIN dev'essere un numero di massimo 4 cifre!");

                await _logicPersone.ResetPin(model);

                return Ok("OK");
            }
            catch (Exception e)
            {
                Log.Error("ResetPin", e);
                return ErrorHandler(e);
            }
        }
    }
}