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

using System.Threading.Tasks;
using System.Web.Mvc;
using PortaleRegione.DTO.Enum;
using PortaleRegione.Gateway;

namespace PortaleRegione.Client.Controllers
{
    /// <summary>
    ///     Controller amministrazione
    /// </summary>
    [Authorize(Roles = RuoliEnum.Amministratore_PEM)]
    [RoutePrefix("admin-panel")]
    public class AdminPanelController : BaseController
    {
        /// <summary>
        ///     Controller per visualizzare i dati degli utenti
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <param name="page">Pagina corrente</param>
        /// <param name="size">Paginazione</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> RiepilogoUtenti(int page = 1, int size = 50)
        {
            return View("RiepilogoUtenti", await ApiGateway.GetPersoneAdmin(page, size));
        }

        [HttpGet]
        [Route("view/{id:int}")]
        public async Task<ActionResult> ViewUtente(int id)
        {
            return View("ViewUtente", await ApiGateway.GetPersonaAdmin(id));
        }
    }
}