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
using System.Data.Entity.Validation;
using System.Globalization;
using System.Security.Claims;
using System.Web.Http;
using Newtonsoft.Json;
using PortaleRegione.DTO.Domain;

namespace PortaleRegione.API.Helpers
{
    /// <summary>
    /// Controller di base
    /// </summary>
    public class BaseApiController : ApiController
    {
        public CurrentUser currentUser => GetUser();

        /// <summary>
        /// Handler per catturare i messaggi di errore
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected IHttpActionResult ErrorHandler(Exception e)
        {
            var message = e.Message;
            if (e.GetType() == typeof(DbEntityValidationException))
            {
                var entityError = e as DbEntityValidationException;
                foreach (var entityValidationError in entityError.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationError.ValidationErrors)
                    {
                        message = validationError.ErrorMessage;
                        break;
                    }

                    break;
                }
            }

            if (e.InnerException != null)
                if (e.InnerException.Source == "EntityFramework")
                    message = e.InnerException.InnerException.Message;

            Console.WriteLine(message);
            return BadRequest(message);
        }

        /// <summary>
        /// Metodo per avere l'utente loggato dal jwt
        /// </summary>
        /// <returns></returns>
        private CurrentUser GetUser()
        {
            var identity = RequestContext.Principal.Identity as ClaimsIdentity;
            var roles = new List<string>();
            PersonaDto personaDto = null;
            foreach (var identityClaim in identity.Claims)
                switch (identityClaim.Type)
                {
                    case ClaimTypes.Role:
                        roles.Add(identityClaim.Value);
                        break;
                    case ClaimTypes.UserData:
                    {
                        personaDto = JsonConvert.DeserializeObject<PersonaDto>(identityClaim.Value);
                        break;
                    }
                }

            var user = new CurrentUser
            {
                Persona = personaDto,
                Ruoli = roles
            };

            return user;
        }
    }

    /// <summary>
    /// Classe di mappaggio sessione utente
    /// </summary>
    public class CurrentUser
    {
        public PersonaDto Persona { get; set; }
        public List<string> Ruoli { get; set; }
    }
}