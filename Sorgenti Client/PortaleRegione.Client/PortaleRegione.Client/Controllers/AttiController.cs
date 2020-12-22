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
using System.Web.Mvc;
using PortaleRegione.Client.Models;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;
using PortaleRegione.DTO.Response;
using PortaleRegione.Gateway;

namespace PortaleRegione.Client.Controllers
{
    /// <summary>
    ///     Controller atti
    /// </summary>
    [Authorize]
    [RoutePrefix("atti")]
    public class AttiController : BaseController
    {
        /// <summary>
        ///     Controller per visualizzare i dati degli atti contenuti in una seduta
        /// </summary>
        /// <param name="id">Guid seduta</param>
        /// <param name="page">Pagina corrente</param>
        /// <param name="size">Paginazione</param>
        /// <returns></returns>
        [Route("{id:guid}")]
        public async Task<ActionResult> RiepilogoAtti(Guid id, ClientModeEnum mode = ClientModeEnum.GRUPPI,
            int page = 1, int size = 50)
        {
            var sedutaInDb = await ApiGateway.GetSeduta(id);

            var model = new AttiViewModel
                {Data = await ApiGateway.GetAtti(id, mode, page, size), Seduta = sedutaInDb};

            if (HttpContext.User.IsInRole(RuoliEnum.Amministratore_PEM) ||
                HttpContext.User.IsInRole(RuoliEnum.Segreteria_Assemblea))
                return View("RiepilogoAtti_Admin", model);

            return View("RiepilogoAtti", model);
        }

        /// <summary>
        ///     Controller per eliminare l'atto
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("{sedutaUId:guid}/delete")]
        public async Task<ActionResult> EliminaAtto(Guid sedutaUId, Guid id)
        {
            await ApiGateway.EliminaAtto(id);
            return RedirectToAction("RiepilogoAtti", new
            {
                id = sedutaUId
            });
        }

        /// <summary>
        ///     Controller per aggiungere un atto
        /// </summary>
        /// <param name="id">Guid seduta di riferimento</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("{id:guid}/new")]
        public async Task<ActionResult> NuovoAtto(Guid id)
        {
            var model = new AttiViewModel
            {
                Assessori = await ApiGateway.GetAssessoriRiferimento(),
                Relatori = await ApiGateway.GetRelatori(null),
                Atto = new AttiFormUpdateModel
                {
                    UIDSeduta = id,
                    IDTipoAtto = (int) TipoAttoEnum.PDL,
                    Notifica_deposito_differita = true
                }
            };

            return View("AttoForm", model);
        }

        /// <summary>
        ///     Controller per modificare un atto
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("edit/{id:guid}")]
        public async Task<ActionResult> ModificaAtto(Guid id)
        {
            var model = new AttiViewModel
            {
                Assessori = await ApiGateway.GetAssessoriRiferimento(),
                Relatori = await ApiGateway.GetRelatori(null),
                Atto = await ApiGateway.GetAttoFormUpdate(id)
            };

            return View("AttoForm", model);
        }

        /// <summary>
        ///     Controller per salvare o modificare l'atto
        /// </summary>
        /// <param name="atto">Modello atto</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("salva")]
        [HttpPost]
        public async Task<ActionResult> SalvaAtto(AttiFormUpdateModel atto)
        {
            try
            {
                AttiDto attoSalvato = null;
                if (atto.UIDAtto == Guid.Empty)
                    attoSalvato = await ApiGateway.SalvaAtto(atto);
                else
                    attoSalvato = await ApiGateway.ModificaAtto(atto);

                return Json(new ClientJsonResponse<AttiDto>
                {
                    entity = attoSalvato,
                    url = Url.Action("RiepilogoAtti", "Atti", new
                    {
                        id = atto.UIDSeduta
                    })
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new ErrorResponse {message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Controller per avere gli articoli
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <returns></returns>
        [Route("articoli")]
        public async Task<ActionResult> GetArticoli(Guid id)
        {
            var articoli = await ApiGateway.GetArticoli(id);
            return Json(articoli, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per creare articoli nell'atto
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <param name="articoli">articoli</param>
        /// <returns></returns>
        [Route("crea-articoli")]
        public async Task<ActionResult> CreaArticoli(Guid id, string articoli)
        {
            await ApiGateway.CreaArticoli(id, articoli);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per elminare un articolo
        /// </summary>
        /// <param name="id">Guid articolo</param>
        /// <returns></returns>
        [Route("elimina-articolo")]
        public async Task<ActionResult> EliminaArticolo(Guid id)
        {
            await ApiGateway.EliminaArticolo(id);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per creare commi agli articoli
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <param name="commi">commi</param>
        /// <returns></returns>
        [Route("crea-commi")]
        public async Task<ActionResult> CreaCommi(Guid id, string commi)
        {
            await ApiGateway.CreaCommi(id, commi);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per avere i commi
        /// </summary>
        /// <param name="id">Guid articolo</param>
        /// <returns></returns>
        [Route("commi")]
        public async Task<ActionResult> GetCommi(Guid id)
        {
            var commi = await ApiGateway.GetCommi(id);
            return Json(commi, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per elminare un comma
        /// </summary>
        /// <param name="id">Guid comma</param>
        /// <returns></returns>
        [Route("elimina-comma")]
        public async Task<ActionResult> EliminaComma(Guid id)
        {
            await ApiGateway.EliminaComma(id);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per avere le lettere
        /// </summary>
        /// <param name="id">Guid comma</param>
        /// <returns></returns>
        [Route("lettere")]
        public async Task<ActionResult> GetLettere(Guid id)
        {
            var lettere = await ApiGateway.GetLettere(id);
            return Json(lettere, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per creare lettere ai commi
        /// </summary>
        /// <param name="id">Guid comma</param>
        /// <param name="lettere">lettere</param>
        /// <returns></returns>
        [Route("crea-lettere")]
        public async Task<ActionResult> CreaLettere(Guid id, string lettere)
        {
            await ApiGateway.CreaLettere(id, lettere);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per elminare una lettera
        /// </summary>
        /// <param name="id">Guid lettera</param>
        /// <returns></returns>
        [Route("elimina-lettera")]
        public async Task<ActionResult> EliminaLettere(Guid id)
        {
            await ApiGateway.EliminaLettera(id);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Controller per salvare o modificare l'atto
        /// </summary>
        /// <param name="atto">Modello atto</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("relatori")]
        [HttpPost]
        public async Task<ActionResult> SalvaRelatoriAtto(AttoRelatoriModel model)
        {
            try
            {
                await ApiGateway.SalvaRelatoriAtto(model);

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new ErrorResponse {message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     Controller per salvare o modificare l'atto
        /// </summary>
        /// <param name="atto">Modello atto</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("abilita-fascicolazione")]
        [HttpPost]
        public async Task<ActionResult> PubblicaFascicolo(PubblicaFascicoloModel model)
        {
            try
            {
                await ApiGateway.PubblicaFascicolo(model);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new ErrorResponse {message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}