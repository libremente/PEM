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
using System.Threading.Tasks;
using ExpressionBuilder.Generics;
using PortaleRegione.Contracts;
using PortaleRegione.DataBase;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;

namespace PortaleRegione.Persistance
{
    /// <summary>
    ///     Implementazione della relativa interfaccia
    /// </summary>
    public class StampeRepository : Repository<STAMPE>, IStampeRepository
    {
        public StampeRepository(PortaleRegioneDbContext context) : base(context)
        {
        }

        public PortaleRegioneDbContext PRContext => Context as PortaleRegioneDbContext;

        public async Task<IEnumerable<STAMPE>> GetAll(PersonaDto persona, int? page, int? size,
            Filter<STAMPE> filtro = null)
        {
            IQueryable<STAMPE> query;
            if (persona.CurrentRole != RuoliIntEnum.Amministratore_PEM &&
                persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                query = PRContext.STAMPE.Where(s => s.UIDUtenteRichiesta == persona.UID_persona);
            else
                query = PRContext.STAMPE.Where(s => true);

            filtro?.BuildExpression(ref query);

            return await query
                .OrderByDescending(s => s.DataRichiesta)
                .Skip((page.Value - 1) * size.Value)
                .Take(size.Value)
                .ToListAsync();
        }

        public async Task<IEnumerable<STAMPE>> GetAll(int? page, int? size)
        {
            return await PRContext
                .STAMPE
                .Where(s => !s.Lock)
                .OrderByDescending(s => s.DataRichiesta)
                .Skip((page.Value - 1) * size.Value)
                .Take(size.Value)
                .ToListAsync();
        }

        public async Task<int> Count(PersonaDto persona, Filter<STAMPE> filtro = null)
        {
            IQueryable<STAMPE> query;
            if (persona.CurrentRole == RuoliIntEnum.Amministratore_PEM ||
                persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                query = PRContext.STAMPE.Where(s => s.CurrentRole == (int) persona.CurrentRole);
            else
                query = PRContext.STAMPE.Where(s => s.UIDUtenteRichiesta == persona.UID_persona);

            filtro?.BuildExpression(ref query);

            return await query.CountAsync();
        }

        public async Task<int> Count()
        {
            return await PRContext
                .STAMPE
                .CountAsync(s => !s.Lock);
        }

        public async Task<STAMPE> Get(Guid stampaUId)
        {
            return await PRContext.STAMPE.FindAsync(stampaUId);
        }
    }
}