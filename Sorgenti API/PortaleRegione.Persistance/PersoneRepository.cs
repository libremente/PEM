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
using ExpressionBuilder.Generics;
using PortaleRegione.Contracts;
using PortaleRegione.DataBase;
using PortaleRegione.Domain;

namespace PortaleRegione.Persistance
{
    /// <summary>
    ///     Implementazione della relativa interfaccia
    /// </summary>
    public class PersoneRepository : Repository<View_UTENTI>, IPersoneRepository
    {
        public PersoneRepository(DbContext context) : base(context)
        {
        }

        public PortaleRegioneDbContext PRContext => Context as PortaleRegioneDbContext;

        public View_UTENTI Get(string login_windows)
        {
            return PRContext.View_UTENTI.SingleOrDefault(a => a.userAD == login_windows);
        }

        public View_UTENTI Get(Guid personaUId)
        {
            return PRContext.View_UTENTI.Find(personaUId);
        }

        public View_UTENTI Get(int personaId)
        {
            return PRContext.View_UTENTI.Find(personaId);
        }

        public IEnumerable<View_UTENTI> GetAll(int page, int size, Filter<View_UTENTI> filtro = null)
        {
            var query = PRContext
                .View_UTENTI
                .Where(u => u.No_Cons == 0 && u.deleted == false)
                .Where(u => u.UID_persona != Guid.Empty);

            filtro?.BuildExpression(ref query);

            query = query.OrderBy(u => u.cognome)
                .ThenBy(u => u.nome);

            return query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public int CountAll(Filter<View_UTENTI> filtro = null)
        {
            var query = PRContext
                .View_UTENTI
                .Where(u => u.No_Cons == 0 && u.deleted == false)
                .Where(u => u.UID_persona != Guid.Empty);

            filtro?.BuildExpression(ref query);

            return query
                .Count();
        }

        public IEnumerable<View_UTENTI> GetAll_DA_CANCELLARE()
        {
            var query = PRContext
                .View_UTENTI
                .Where(u => u.No_Cons == 0 && u.deleted == false)
                .Where(u => u.UID_persona != Guid.Empty)
                .OrderBy(u => u.cognome)
                .ThenBy(u => u.nome);

            return query
                .ToList();
        }

        public IEnumerable<View_UTENTI> GetAssessoriRiferimento(int id_legislatura)
        {
            var organi = PRContext.organi
                .Where(o =>
                    o.deleted == false
                    && o.id_tipo_organo == 4
                    && o.id_legislatura == id_legislatura)
                .ToList().Select(o => o.id_organo);

            var query = PRContext
                .join_persona_organo_carica
                .Where(jpoc =>
                    jpoc.deleted == false
                    && jpoc.id_legislatura == id_legislatura
                    && organi.Contains(jpoc.id_organo)
                    && jpoc.data_inizio <= DateTime.Now
                    && (jpoc.data_fine >= DateTime.Now || jpoc.data_fine == null))
                .Join(PRContext.join_persona_AD,
                    p => p.id_persona,
                    a => a.id_persona,
                    (p, a) => a.UID_persona)
                .Join(PRContext.View_UTENTI,
                    u => u,
                    p => p.UID_persona,
                    (u, p) => p);
            return query.ToList()
                .Distinct()
                .OrderBy(p => p.cognome)
                .ThenBy(p => p.nome)
                .ToList();
        }

        public IEnumerable<View_UTENTI> GetRelatori(Guid? attoUId)
        {
            if (attoUId == Guid.Empty)
            {
                var query = PRContext
                    .View_UTENTI
                    .Where(u =>
                        u.legislatura_attuale == true
                        && u.No_Cons == 0);

                return query.ToList();
            }
            else
            {
                var query = PRContext
                    .ATTI_RELATORI
                    .Where(a => a.UIDAtto == attoUId)
                    .Join(
                        PRContext.View_UTENTI,
                        a => a.UIDPersona,
                        p => p.UID_persona,
                        (a, p) => p);

                return query.ToList();
            }
        }

        public bool IsRelatore(Guid personaUId, Guid attoUId)
        {
            return PRContext
                .ATTI_RELATORI
                .Any(ar => ar.UIDAtto == attoUId && ar.UIDPersona == personaUId);
        }

        public bool IsAssessore(Guid personaUId, Guid attoUId)
        {
            return PRContext
                .ATTI
                .Any(a => a.UIDAtto == attoUId && a.UIDAssessoreRiferimento == personaUId);
        }

        public string GetCarica(Guid personaUId)
        {
            var query = PRContext
                .Database
                .SqlQuery<string>($"SELECT dbo.get_CaricaGiunta_from_UIDpersona('{personaUId}')");
            return query.FirstOrDefault();
        }

        public View_PINS GetPin(Guid personaUId)
        {
            return PRContext
                .View_PINS
                .SingleOrDefault(p => p.UID_persona == personaUId
                                      && !p.Al.HasValue);
        }

        public IEnumerable<View_UTENTI> GetConsiglieri(int id_legislatura)
        {
            var organi = PRContext.organi
                .Where(o =>
                    o.deleted == false
                    && o.id_tipo_organo != 4
                    && o.id_legislatura == id_legislatura)
                .ToList().Select(o => o.id_organo);

            var query = PRContext
                .join_persona_organo_carica
                .Where(jpoc =>
                    jpoc.deleted == false
                    && jpoc.id_legislatura == id_legislatura
                    && organi.Contains(jpoc.id_organo)
                    && jpoc.data_inizio <= DateTime.Now
                    && (jpoc.data_fine >= DateTime.Now || jpoc.data_fine == null))
                .Join(PRContext.join_persona_AD,
                    p => p.id_persona,
                    a => a.id_persona,
                    (p, a) => a.UID_persona)
                .Join(PRContext.View_UTENTI,
                    u => u,
                    p => p.UID_persona,
                    (u, p) => p);
            var result = query
                .ToList()
                .Distinct()
                .OrderBy(p => p.cognome)
                .ThenBy(p => p.nome)
                .ToList();

            return result;
        }

        public void SavePin(Guid personaUId, string nuovo_pin, bool reset)
        {
            var persona = Get(personaUId);
            var no_cons = Convert.ToBoolean(persona.No_Cons);
            var table = no_cons ? "PINS_NoCons" : "PINS";
            PRContext.Database.ExecuteSqlCommand(
                $"UPDATE {table} SET Al=GETDATE() WHERE Al IS NULL AND UID_persona='{personaUId}'");
            PRContext.Database.ExecuteSqlCommand(
                $"INSERT INTO {table} (UID_persona,PIN,RichiediModificaPIN) VALUES ('{personaUId}','{nuovo_pin}',{(reset ? 1 : 0)})");
        }

        public IEnumerable<View_Composizione_GiuntaRegionale> GetGiuntaRegionale()
        {
            try
            {
                var query = PRContext.View_Composizione_GiuntaRegionale;
                return query.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IEnumerable<UTENTI_NoCons> GetSegreteriaGiuntaRegionale(int id, bool notifica_firma,
            bool notifica_deposito)
        {
            var query = PRContext
                .UTENTI_NoCons
                .SqlQuery($"Select * from UTENTI_NoCons where id_gruppo_politico_rif={id}");

            if (notifica_firma)
                return query.Where(u => u.notifica_firma).ToList();
            if (notifica_deposito)
                return query.Where(u => u.notifica_deposito).ToList();

            return query.ToList();
        }
    }
}