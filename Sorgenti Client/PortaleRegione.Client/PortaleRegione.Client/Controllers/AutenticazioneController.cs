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
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using PortaleRegione.DTO.Autenticazione;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Response;
using PortaleRegione.Gateway;

namespace PortaleRegione.Client.Controllers
{
    /// <summary>
    ///     Controller di autenticazione
    /// </summary>
    [Authorize]
    public class AutenticazioneController : BaseController
    {
        [AllowAnonymous]
        [Route("login")]
        public ActionResult FormAutenticazione()
        {
            return View(new LoginRequest());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginRequest model, string returnUrl)
        {
            if (!ModelState.IsValid) return View("FormAutenticazione", model);
            LoginResponse response;
            try
            {
                response = await ApiGateway.Login(model.Username, model.Password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                model.MessaggioErrore = e.Message;
                return View("FormAutenticazione", model);
            }

            SalvaDatiInCookies(response.persona, response.jwt, model.Username);

            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            return RedirectToAction("RiepilogoSedute", "Sedute");
        }

        [Authorize]
        [HttpGet]
        [Route("cambio-ruolo")]
        public async Task<ActionResult> CambioRuolo(RuoliIntEnum ruolo, string returnUrl)
        {
            LoginResponse response;
            try
            {
                response = await ApiGateway.CambioRuolo(ruolo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpNotFound(e.Message);
            }

            SalvaDatiInCookies(response.persona, response.jwt, response.persona.userAD.Replace(@"CONSIGLIO\", ""));

            //if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            return RedirectToAction("RiepilogoSedute", "Sedute");
        }

        [Authorize]
        [HttpGet]
        [Route("cambio-gruppo")]
        public async Task<ActionResult> CambioGruppo(int gruppo, string returnUrl)
        {
            LoginResponse response;
            try
            {
                response = await ApiGateway.CambioGruppo(gruppo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return HttpNotFound(e.Message);
            }

            SalvaDatiInCookies(response.persona, response.jwt, response.persona.userAD.Replace(@"CONSIGLIO\", ""));

            //if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            return RedirectToAction("RiepilogoSedute", "Sedute");
        }

        private void SalvaDatiInCookies(PersonaDto persona, string jwt, string username)
        {
            string jwt1 = string.Empty, jwt2 = string.Empty, jwt3 = string.Empty;
            SliceBy3(jwt, ref jwt1, ref jwt2, ref jwt3);

            var gruppi = persona.GruppiAdmin.Any() ? JsonConvert.SerializeObject(persona.GruppiAdmin) : "";
            var groupsTicket = new FormsAuthenticationTicket
            (
                1, "gruppi", DateTime.Now, DateTime.Now.AddMinutes(30), true, gruppi
            );
            var gTicket = FormsAuthentication.Encrypt(groupsTicket);
            var gCookie = new HttpCookie("GCookies", gTicket) {HttpOnly = true};
            Response.Cookies.Add(gCookie);

            persona.GruppiAdmin = null;

            var persona_json = JsonConvert.SerializeObject(persona);
            string p1 = string.Empty, p2 = string.Empty, p3 = string.Empty;

            SliceBy3(persona_json, ref p1, ref p2, ref p3);

            #region USER DATA

            var authTicket1 = new FormsAuthenticationTicket
            (
                1, $"{username}1", DateTime.Now, DateTime.Now.AddMinutes(30), true, p1
            );
            var authTicket2 = new FormsAuthenticationTicket
            (
                1, $"{username}2", DateTime.Now, DateTime.Now.AddMinutes(30), true, p2
            );
            var authTicket3 = new FormsAuthenticationTicket
            (
                1, $"{username}3", DateTime.Now, DateTime.Now.AddMinutes(30), true, p3
            );

            var enTicket1 = FormsAuthentication.Encrypt(authTicket1);
            var faCookie1 = new HttpCookie("PRCookies1", enTicket1) {HttpOnly = true};
            Response.Cookies.Add(faCookie1);
            var enTicket2 = FormsAuthentication.Encrypt(authTicket2);
            var faCookie2 = new HttpCookie("PRCookies2", enTicket2) {HttpOnly = true};
            Response.Cookies.Add(faCookie2);
            var enTicket3 = FormsAuthentication.Encrypt(authTicket3);
            var faCookie3 = new HttpCookie("PRCookies3", enTicket3) {HttpOnly = true};
            Response.Cookies.Add(faCookie3);

            #endregion

            #region JWT DATA

            var securetyTicket1 = new FormsAuthenticationTicket
            (
                1, "token_jwt1", DateTime.Now, DateTime.Now.AddMinutes(30), true, jwt1
            );
            var securetyTicket2 = new FormsAuthenticationTicket
            (
                1, "token_jwt2", DateTime.Now, DateTime.Now.AddMinutes(30), true, jwt2
            );
            var securetyTicket3 = new FormsAuthenticationTicket
            (
                1, "token_jwt3", DateTime.Now, DateTime.Now.AddMinutes(30), true, jwt3
            );

            var sTicket1 = FormsAuthentication.Encrypt(securetyTicket1);
            var sCookie1 = new HttpCookie("SCookies1", sTicket1) {HttpOnly = true};
            Response.Cookies.Add(sCookie1);
            var sTicket2 = FormsAuthentication.Encrypt(securetyTicket2);
            var sCookie2 = new HttpCookie("SCookies2", sTicket2) {HttpOnly = true};
            Response.Cookies.Add(sCookie2);
            var sTicket3 = FormsAuthentication.Encrypt(securetyTicket3);
            var sCookie3 = new HttpCookie("SCookies3", sTicket3) {HttpOnly = true};
            Response.Cookies.Add(sCookie3);

            #endregion

            FormsAuthentication.SetAuthCookie(username, true);
            BaseGateway.access_token = jwt;
        }

        [HttpPost]
        public ActionResult Logout()
        {
            if (Response != null)
            {
                if (Response.Cookies.AllKeys.Contains("PRCookies1"))
                    Response.Cookies.Remove("PRCookies1");
                if (Response.Cookies.AllKeys.Contains("PRCookies2"))
                    Response.Cookies.Remove("PRCookies2");
                if (Response.Cookies.AllKeys.Contains("PRCookies3"))
                    Response.Cookies.Remove("PRCookies3");
                if (Response.Cookies.AllKeys.Contains("SCookies1"))
                    Response.Cookies.Remove("SCookies1");
                if (Response.Cookies.AllKeys.Contains("SCookies2"))
                    Response.Cookies.Remove("SCookies2");
                if (Response.Cookies.AllKeys.Contains("SCookies3"))
                    Response.Cookies.Remove("SCookies3");
            }

            FormsAuthentication.SignOut();
            BaseGateway.access_token = string.Empty;
            return RedirectToAction("FormAutenticazione", "Autenticazione");
        }
    }
}