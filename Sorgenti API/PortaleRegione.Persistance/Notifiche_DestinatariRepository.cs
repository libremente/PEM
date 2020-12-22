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
using System.Linq;
using PortaleRegione.Contracts;
using PortaleRegione.DataBase;
using PortaleRegione.Domain;

namespace PortaleRegione.Persistance
{
    /// <summary>
    ///     Implementazione della relativa interfaccia
    /// </summary>
    public class Notifiche_DestinatariRepository : Repository<NOTIFICHE_DESTINATARI>, INotifiche_DestinatariRepository
    {
        public Notifiche_DestinatariRepository(PortaleRegioneDbContext context) : base(context)
        {
        }

        public PortaleRegioneDbContext PRContext => Context as PortaleRegioneDbContext;

        public NOTIFICHE_DESTINATARI Get(long notificaId, Guid personaUId)
        {
            return PRContext.NOTIFICHE_DESTINATARI.SingleOrDefault(nd =>
                nd.UIDNotifica == notificaId && nd.UIDPersona == personaUId);
        }

        public bool ExistDestinatarioNotifica(Guid emendamentoUId, Guid personaUId)
        {
            var listaDestinatariEM = PRContext
                .NOTIFICHE_DESTINATARI
                .Where(nd => nd.NOTIFICHE.UIDEM == emendamentoUId && nd.UIDPersona == personaUId)
                .ToList();

            return listaDestinatariEM.Any();
        }
    }
}