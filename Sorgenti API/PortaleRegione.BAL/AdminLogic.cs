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
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using ExpressionBuilder.Generics;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Request;
using PortaleRegione.Logger;

namespace PortaleRegione.BAL
{
    public class AdminLogic : BaseLogic
    {
        private readonly PersoneLogic _logicPersona;
        private readonly IUnitOfWork _unitOfWork;

        public AdminLogic(IUnitOfWork unitOfWork, PersoneLogic logicPersona)
        {
            _unitOfWork = unitOfWork;
            _logicPersona = logicPersona;
        }

        #region GetPersoneIn_DB

        public async Task<IEnumerable<PersonaDto>> GetPersoneIn_DB(BaseRequest<PersonaDto> model)
        {
            try
            {
                var queryFilter = new Filter<View_UTENTI>();
                queryFilter.ImportStatements(model.filtro);

                var listaPersone = (await _unitOfWork
                        .Persone
                        .GetAll(model.page,
                            model.size,
                            queryFilter))
                    .Select(Mapper.Map<View_UTENTI, PersonaDto>);

                return listaPersone;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetPersoneIn_DB", e);
                throw e;
            }
        }

        #endregion

        #region GetGroups

        public List<string> GetADGroups(string userName, string simplefilter = "")
        {
            var result = new List<string>();
            try
            {
                var wi = new WindowsIdentity(userName);

                foreach (var group in wi.Groups)
                    try
                    {
                        if (simplefilter != "")
                        {
                            if (group.Translate(typeof(NTAccount)).ToString().Contains(simplefilter))
                                result.Add(group.Translate(typeof(NTAccount)).ToString().Replace(@"CONSIGLIO\", ""));
                        }
                        else
                        {
                            result.Add(group.Translate(typeof(NTAccount)).ToString().Replace(@"CONSIGLIO\", ""));
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                result.Sort();
                return result;
            }
            catch (Exception ex)
            {
                var result_vuoto = new List<string>();
                result_vuoto.Add("");
                return result_vuoto;
            }
        }

        #endregion

        #region Count

        public async Task<int> Count(BaseRequest<PersonaDto> model)
        {
            try
            {
                var queryFilter = new Filter<View_UTENTI>();
                queryFilter.ImportStatements(model.filtro);

                return await _unitOfWork
                    .Persone
                    .CountAll(queryFilter);
            }
            catch (Exception e)
            {
                Log.Error("Logic - CountAll", e);
                throw e;
            }
        }

        #endregion

        #region GetPin

        /// <summary>
        ///     Controlla il pin della persona
        /// </summary>
        /// <param name="persona"></param>
        public async Task<StatoPinEnum> CheckPin(PersonaDto persona)
        {
            var pinInDb = await _logicPersona.GetPin(persona);
            if (pinInDb == null)
                return StatoPinEnum.NESSUNO;

            return pinInDb.RichiediModificaPIN ? StatoPinEnum.RESET : StatoPinEnum.VALIDO;
        }

        #endregion

        #region GetRuoliAD

        public async Task<IEnumerable<RuoliDto>> GetRuoliAD(bool SoloRuoliGiunta)
        {
            var listaRuoli = await _unitOfWork.Ruoli.GetAll(SoloRuoliGiunta);
            return listaRuoli.Select(Mapper.Map<RUOLI, RuoliDto>);
        }

        #endregion

        #region GetGruppiPoliticiAD

        public async Task<IEnumerable<GruppoAD_Dto>> GetGruppiPoliticiAD(bool SoloRuoliGiunta)
        {
            var listaGruppiAD = await _unitOfWork
                .Gruppi
                .GetGruppiPoliticiAD(
                    await _unitOfWork.Legislature.Legislatura_Attiva(),
                    SoloRuoliGiunta);
            return listaGruppiAD.Select(Mapper.Map<JOIN_GRUPPO_AD, GruppoAD_Dto>);
        }

        #endregion

        #region GetPersona

        public async Task<PersonaDto> GetPersona(int id)
        {
            return await _logicPersona.GetPersona(id);
        }

        public async Task<PersonaDto> GetPersona(Guid id)
        {
            return await _logicPersona.GetPersona(id);
        }

        public async Task<PersonaDto> GetPersona(PersonaDto persona, List<string> gruppi_utente)
        {
            var ruoli_utente = (await _unitOfWork.Ruoli.RuoliUtente(gruppi_utente)).ToList();
            if (ruoli_utente.Any())
            {
                persona.Ruoli = ruoli_utente.Select(Mapper.Map<RUOLI, RuoliDto>);
                persona.CurrentRole = (RuoliIntEnum) ruoli_utente[0].IDruolo;
            }
            else
            {
                persona.CurrentRole = RuoliIntEnum.Utente;
            }

            if (gruppi_utente.Any())
            {
                persona.Gruppo = await _unitOfWork.Gruppi.GetGruppoPersona(gruppi_utente, persona.IsGiunta());
                persona.Gruppi = gruppi_utente.Aggregate((i, j) => i + "; " + j);
            }

            var gruppiUtente_AD = GetADGroups(persona.userAD.Replace(@"CONSIGLIO\", ""));
            if (gruppiUtente_AD.Any())
                persona.GruppiAD = gruppiUtente_AD.Aggregate((i, j) => i + "; " + j);

            persona.Stato_Pin = await CheckPin(persona);

            return persona;
        }

        #endregion
    }
}