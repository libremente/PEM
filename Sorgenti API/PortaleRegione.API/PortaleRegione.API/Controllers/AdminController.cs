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
using System.Threading.Tasks;
using System.Web.Http;
using PortaleRegione.API.Helpers;
using PortaleRegione.API.it.lombardia.regione.consiglio.intranet;
using PortaleRegione.BAL;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Request;
using PortaleRegione.DTO.Response;
using PortaleRegione.Logger;

namespace PortaleRegione.API.Controllers
{
    /// <summary>
    ///     Controller per endpoint di amministrazione
    /// </summary>
    [Authorize(Roles = RuoliEnum.Amministratore_PEM)]
    [RoutePrefix("admin")]
    public class AdminController : BaseApiController
    {
        private readonly AdminLogic _logic;

        public AdminController(PersoneLogic logicPersone, AdminLogic logic) : base(logicPersone)
        {
            _logic = logic;
        }

        /// <summary>
        ///     Endpoint per avere gli utenti nel db
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> GetUtenti(BaseRequest<PersonaDto> model)
        {
            try
            {
                var results = await _logic.GetPersoneIn_DB(model);

                var intranetAdService = new proxyAD();
                var personaDtos = results.ToList();
                personaDtos.ToList().ForEach(async persona =>
                {
                    var gruppiUtente_PEM = new List<string>(intranetAdService.GetGroups(
                        persona.userAD.Replace(@"CONSIGLIO\", ""), "PEM_", AppSettingsConfiguration.TOKEN_R));
                    if (gruppiUtente_PEM.Any())
                        persona.Gruppi = gruppiUtente_PEM.Aggregate((i, j) => i + "; " + j);

                    var gruppiUtente_AD = _logic.GetADGroups(persona.userAD.Replace(@"CONSIGLIO\", ""));
                    if (gruppiUtente_AD.Any())
                        persona.GruppiAD = gruppiUtente_AD.Aggregate((i, j) => i + "; " + j);

                    persona.Stato_Pin = await _logic.CheckPin(persona);
                });

                return Ok(new BaseResponse<PersonaDto>(
                    model.page,
                    model.size,
                    personaDtos,
                    model.filtro,
                    await _logic.Count(model),
                    Request.RequestUri));
            }
            catch (Exception e)
            {
                Log.Error("GetUtenti", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere un utente nel db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("view/{id:guid}")]
        public async Task<IHttpActionResult> GetUtente(Guid id)
        {
            try
            {
                var persona = await _logic.GetPersona(id);

                var intranetAdService = new proxyAD();

                var lRuoli = new List<string>();
                var Gruppi_Utente = new List<string>(intranetAdService.GetGroups(
                    persona.userAD.Replace(@"CONSIGLIO\", ""), "PEM_", AppSettingsConfiguration.TOKEN_R));

                foreach (var group in Gruppi_Utente)
                    lRuoli.Add($"CONSIGLIO\\{group}");

                var personaResult = await _logic.GetPersona(persona, lRuoli);

                return Ok(personaResult);
            }
            catch (Exception e)
            {
                Log.Error("GetUtente", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere i ruoli AD disponibili
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ad/ruoli")]
        public async Task<IHttpActionResult> GetRuoliAD()
        {
            try
            {
                var currentUser = await _logic.GetPersona((await GetSession()).UID_persona);
                var ruoli = await _logic.GetRuoliAD(currentUser.CurrentRole == RuoliIntEnum.Amministratore_PEM);

                return Ok(ruoli);
            }
            catch (Exception e)
            {
                Log.Error("GetRuoli", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere i gruppi politici AD disponibili
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ad/gruppi-politici")]
        public async Task<IHttpActionResult> GetGruppiPoliticiAD()
        {
            try
            {
                var gruppiPoliticiAD =
                    await _logic.GetGruppiPoliticiAD(
                        (await GetSession()).CurrentRole == RuoliIntEnum.Amministratore_PEM);

                return Ok(gruppiPoliticiAD);
            }
            catch (Exception e)
            {
                Log.Error("GetGruppiPoliticiAD", e);
                return ErrorHandler(e);
            }
        }
    }
}