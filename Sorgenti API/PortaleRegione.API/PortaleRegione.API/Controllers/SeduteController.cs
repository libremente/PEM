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
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using PortaleRegione.API.Helpers;
using PortaleRegione.BAL;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Request;
using PortaleRegione.Logger;

namespace PortaleRegione.API.Controllers
{
    /// <summary>
    ///     Controller per gestire le sedute
    /// </summary>
    [Authorize]
    [RoutePrefix("sedute")]
    public class SeduteController : BaseApiController
    {
        private readonly SeduteLogic _logic;

        public SeduteController(PersoneLogic logicPersone, SeduteLogic logic) : base(logicPersone)
        {
            _logic = logic;
        }

        /// <summary>
        ///     Endpoint per avere il riepilogo delle sedute
        /// </summary>
        /// <param name="model">Modello richiesta generico con paginazione</param>
        /// <returns></returns>
        [HttpPost]
        [Route("view")]
        public IHttpActionResult GetSedute(BaseRequest<SeduteDto> model)
        {
            try
            {
                return Ok(_logic.GetSedute(model, Request.RequestUri));
            }
            catch (Exception e)
            {
                Log.Error("GetSedute", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere una seduta precisa
        /// </summary>
        /// <param name="id">Guid seduta</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSeduta(Guid id)
        {
            try
            {
                var result = _logic.GetSeduta(id);

                if (result == null)
                    return NotFound();

                return Ok(Mapper.Map<SEDUTE, SeduteDto>(result));
            }
            catch (Exception e)
            {
                Log.Error("GetSeduta", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per eliminare virtualmente una seduta
        /// </summary>
        /// <param name="id">Guid seduta</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteSeduta(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest();

                var sedutaInDb = _logic.GetSeduta(id);

                if (sedutaInDb == null)
                    return NotFound();

                await _logic.DeleteSeduta(Mapper.Map<SEDUTE, SeduteDto>(sedutaInDb), SessionManager.Persona);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("DeleteSeduta", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per aggiungere una seduta a database
        /// </summary>
        /// <param name="sedutaDto">Modello seduta da inserire</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpPost]
        public async Task<IHttpActionResult> NuovaSeduta(SeduteDto sedutaDto)
        {
            try
            {
                var seduta =
                    Mapper.Map<SEDUTE, SeduteDto>(await _logic.NuovaSeduta(Mapper.Map<SeduteDto, SEDUTE>(sedutaDto),
                        SessionManager.Persona));
                return Created(new Uri(Request.RequestUri + "/" + seduta.UIDSeduta), seduta);
            }
            catch (Exception e)
            {
                Log.Error("NuovaSeduta", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per modificare una seduta
        /// </summary>
        /// <param name="sedutaDto">Modello seduta da modificare</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpPut]
        public async Task<IHttpActionResult> ModificaSeduta(SeduteFormUpdateDto sedutaDto)
        {
            try
            {
                var sedutaInDb = _logic.GetSeduta(sedutaDto.UIDSeduta);

                if (sedutaInDb == null)
                    return NotFound();

                await _logic.ModificaSeduta(sedutaDto, SessionManager.Persona);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("ModificaSeduta", e);
                return ErrorHandler(e);
            }
        }

        #region ### FILTRI ###

        /// <summary>
        ///     Endpoint per avere la lista delle legislature disponibili
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("legislature")]
        public async Task<IHttpActionResult> GetLegislature()
        {
            return Ok(_logic.GetLegislature());
        }

        #endregion
    }
}