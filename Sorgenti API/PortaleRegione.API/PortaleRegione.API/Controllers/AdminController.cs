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
using System.Threading.Tasks;
using System.Web.Http;
using PortaleRegione.API.Helpers;
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

        /// <summary>
        ///     ctor
        /// </summary>
        /// <param name="logic"></param>
        public AdminController(AdminLogic logic)
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
                var results = _logic.GetPersoneIn_DB(model);

                return Ok(new BaseResponse<PersonaDto>(
                    model.page,
                    model.size,
                    results,
                    model.filtro,
                    _logic.Count(model),
                    Request.RequestUri));
            }
            catch (Exception e)
            {
                Log.Error("GetUtenti", e);
                return ErrorHandler(e);
            }
        }
    }
}