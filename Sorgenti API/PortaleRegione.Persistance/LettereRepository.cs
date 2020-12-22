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
using System.Data.Entity;
using System.Linq;
using PortaleRegione.Contracts;
using PortaleRegione.DataBase;
using PortaleRegione.Domain;

namespace PortaleRegione.Persistance
{
    /// <summary>
    ///     Implementazione della relativa interfaccia
    /// </summary>
    public class LettereRepository : Repository<LETTERE>, ILettereRepository
    {
        public LettereRepository(DbContext context) : base(context)
        {
        }

        public PortaleRegioneDbContext PRContext => Context as PortaleRegioneDbContext;

        public bool CheckIfLetteraExists(Guid commaUId, string lettera)
        {
            return PRContext
                .LETTERE
                .Any(a => a.UIDComma == commaUId && a.Lettera.Contains(lettera));
        }

        public LETTERE GetLettera(Guid lettaraUId)
        {
            return PRContext.LETTERE.Find(lettaraUId);
        }

        public IEnumerable<LETTERE> GetLettere(Guid commaUId)
        {
            return PRContext
                .LETTERE
                .Where(l => l.UIDComma == commaUId)
                .OrderBy(l => l.Ordine)
                .ToList();
        }

        public int OrdineLettera(Guid commaUId)
        {
            var list = PRContext
                .LETTERE
                .Where(a => a.UIDComma == commaUId)
                .OrderByDescending(a => a.Ordine)
                .Take(1)
                .ToList();

            return list.Any() ? list[0].Ordine + 1 : 1;
        }
    }
}