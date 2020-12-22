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
    public class CommiRepository : Repository<COMMI>, ICommiRepository
    {
        public CommiRepository(DbContext context) : base(context)
        {
        }

        public PortaleRegioneDbContext PRContext => Context as PortaleRegioneDbContext;

        public bool CheckIfCommiExists(Guid articoloUId, string comma)
        {
            return PRContext
                .COMMI
                .Any(c => c.UIDArticolo == articoloUId && c.Comma.Contains(comma));
        }

        public IEnumerable<COMMI> GetCommi(Guid articoloUId)
        {
            return PRContext
                .COMMI
                .Where(c => c.UIDArticolo == articoloUId)
                .OrderBy(c => c.Ordine)
                .ToList();
        }

        public COMMI GetComma(Guid commaUId)
        {
            return PRContext.COMMI.Find(commaUId);
        }

        public int OrdineComma(Guid articoloUId)
        {
            var list = PRContext
                .COMMI
                .Where(c => c.UIDArticolo == articoloUId)
                .OrderByDescending(c => c.Ordine)
                .Take(1)
                .ToList();

            return list.Any() ? list[0].Ordine.GetValueOrDefault() + 1 : 1;
        }
    }
}