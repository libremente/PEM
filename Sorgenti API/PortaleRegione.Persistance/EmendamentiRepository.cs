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
using PortaleRegione.BAL;
using PortaleRegione.Contracts;
using PortaleRegione.DataBase;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;

namespace PortaleRegione.Persistance
{
    /// <summary>
    ///     Implementazione della relativa interfaccia
    /// </summary>
    public class EmendamentiRepository : Repository<EM>, IEmendamentiRepository
    {
        public EmendamentiRepository(DbContext context) : base(context)
        {
        }

        public PortaleRegioneDbContext PRContext => Context as PortaleRegioneDbContext;

        /// <summary>
        ///     Conteggio emendamenti nell'atto
        /// </summary>
        /// <param name="attoUId"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        public int Count(Guid attoUId, PersonaDto persona, CounterEmendamentiEnum counter_emendamenti, int CLIENT_MODE,
            Filter<EM> filtro = null)
        {
            var query = PRContext.EM
                .Where(em =>
                    em.UIDAtto == attoUId
                    && !em.Eliminato);
            if (CLIENT_MODE == (int) ClientModeEnum.TRATTAZIONE)
            {
                query = query.Where(em => em.IDStato >= (int) StatiEnum.Depositato);
            }
            else
            {
                query = query.Where(em => em.IDStato != (int) StatiEnum.Bozza_Riservata
                                  || em.IDStato == (int) StatiEnum.Bozza_Riservata
                                  && (em.UIDPersonaCreazione == persona.UID_persona
                                      || em.UIDPersonaProponente == persona.UID_persona));

                if (persona.CurrentRole != RuoliIntEnum.Amministratore_PEM
                    && persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                    query = query
                        .Where(em => em.id_gruppo == persona.Gruppo.id_gruppo);

                if (persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                    query = query.Where(em =>
                        !string.IsNullOrEmpty(em.DataDeposito) ||
                        em.idRuoloCreazione == (int) RuoliIntEnum.Segreteria_Assemblea);
            }

            filtro?.BuildExpression(ref query);

            switch (counter_emendamenti)
            {
                case CounterEmendamentiEnum.NONE:
                    return query.ToList().Count;
                case CounterEmendamentiEnum.EM:
                    if (persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                        return query.Count(e => !string.IsNullOrEmpty(e.N_EM) && string.IsNullOrEmpty(e.N_SUBEM));
                    else
                        return query.Count(e => string.IsNullOrEmpty(e.N_SUBEM));
                case CounterEmendamentiEnum.SUB_EM:
                    if (persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                        return query.Count(e => string.IsNullOrEmpty(e.N_EM));
                    else
                        return query.Count(e => string.IsNullOrEmpty(e.N_EM) && !string.IsNullOrEmpty(e.N_SUBEM));
                default:
                    return 0;
            }
        }

        public int Count(string query)
        {
            return PRContext
                .EM
                .SqlQuery(query)
                .Count();
        }

        #region GET

        /// <summary>
        ///     Riepilogo emendamenti
        /// </summary>
        /// <param name="attoUId"></param>
        /// <param name="persona"></param>
        /// <param name="ordine"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IEnumerable<Guid> GetAll(Guid attoUId, PersonaDto persona, OrdinamentoEnum ordine, int? page,
            int? size, int CLIENT_MODE, Filter<EM> filtro = null)
        {
            var query = PRContext.EM
                .Where(em =>
                    em.UIDAtto == attoUId
                    && !em.Eliminato);
            if (CLIENT_MODE == (int) ClientModeEnum.TRATTAZIONE)
            {
                query = query.Where(em => em.IDStato >= (int) StatiEnum.Depositato);
            }
            else
            {
                query = query.Where(em => em.IDStato != (int) StatiEnum.Bozza_Riservata
                                  || em.IDStato == (int) StatiEnum.Bozza_Riservata
                                  && (em.UIDPersonaCreazione == persona.UID_persona
                                      || em.UIDPersonaProponente == persona.UID_persona));
                if (persona.CurrentRole != RuoliIntEnum.Amministratore_PEM
                    && persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                    query = query
                        .Where(em => em.id_gruppo == persona.Gruppo.id_gruppo);

                if (persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                    query = query.Where(em =>
                        !string.IsNullOrEmpty(em.DataDeposito) ||
                        em.idRuoloCreazione == (int) RuoliIntEnum.Segreteria_Assemblea);
            }

            filtro?.BuildExpression(ref query);

            switch (ordine)
            {
                case OrdinamentoEnum.Presentazione:
                    query = query.OrderBy(em => em.SubProgressivo).ThenBy(em => em.Timestamp);
                    break;
                case OrdinamentoEnum.Votazione:
                    query = query.OrderBy(em => em.OrdineVotazione);
                    break;
                default:
                    query = query.OrderBy(em => em.IDStato)
                        .ThenBy(em => em.Progressivo)
                        .ThenBy(em => em.SubProgressivo);
                    break;
            }

            return query
                .Skip((page.Value - 1) * size.Value)
                .Take(size.Value)
                .Select(em => em.UIDEM)
                .ToList();
        }

        /// <summary>
        ///     Esegue query emendamenti per le stampe
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<EM> GetAll(EmendamentiByQueryModel model)
        {
            return PRContext
                .EM
                .SqlQuery(model.Query)
                .Skip((model.page - 1) * model.size)
                .Take(model.size)
                .ToList();
        }

        /// <summary>
        ///     Ritorna la query emednamenti da stampare
        /// </summary>
        /// <param name="attoUId"></param>
        /// <param name="persona"></param>
        /// <param name="ordine"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        public string GetAll_Query(Guid attoUId, PersonaDto persona, OrdinamentoEnum ordine, Filter<EM> filtro = null)
        {
            var query = PRContext.EM
                .Where(em =>
                    em.UIDAtto == attoUId
                    && !em.Eliminato
                    && (em.IDStato != (int) StatiEnum.Bozza
                        || em.IDStato == (int) StatiEnum.Bozza
                        && (em.UIDPersonaCreazione == persona.UID_persona
                            || em.UIDPersonaProponente == persona.UID_persona)));

            if (persona.CurrentRole != RuoliIntEnum.Amministratore_PEM
                && persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                query = query
                    .Where(em => em.id_gruppo == persona.Gruppo.id_gruppo);
            else if (ordine == OrdinamentoEnum.Default) ordine = OrdinamentoEnum.Presentazione;

            if (persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                query = query.Where(em =>
                    !string.IsNullOrEmpty(em.DataDeposito) ||
                    em.idRuoloCreazione == (int) RuoliIntEnum.Segreteria_Assemblea);

            filtro?.BuildExpression(ref query);

            switch (ordine)
            {
                case OrdinamentoEnum.Presentazione:
                    query = query.OrderBy(em => em.SubProgressivo).ThenBy(em => em.Timestamp);
                    break;
                case OrdinamentoEnum.Votazione:
                    query = query.OrderBy(em => em.OrdineVotazione);
                    break;
                case OrdinamentoEnum.Default:
                    query = query.OrderBy(em => em.IDStato)
                        .ThenBy(em => em.Progressivo)
                        .ThenBy(em => em.Timestamp);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ordine), ordine, null);
            }

            var sql = query.ToTraceQuery();
            return sql;
        }

        /// <summary>
        ///     Singolo emendamento
        /// </summary>
        /// <param name="emendamentoUId"></param>
        /// <returns></returns>
        public EM Get(Guid emendamentoUId)
        {
            return PRContext.EM.Find(emendamentoUId);
        }

        /// <summary>
        ///     Singolo emendamento
        /// </summary>
        /// <param name="emendamentoUId"></param>
        /// <returns></returns>
        public EM Get(string emendamentoUId)
        {
            var guidId = new Guid(emendamentoUId);
            return Get(guidId);
        }

        /// <summary>
        ///     Etichetta di deposito
        /// </summary>
        /// <param name="attoUId"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public int GetEtichetta(Guid attoUId, bool sub)
        {
            var query = PRContext.EM
                .Where(em => em.UIDAtto == attoUId
                             && em.Eliminato == false);
            return sub
                ? query.Count(e => string.IsNullOrEmpty(e.N_EM) && !string.IsNullOrEmpty(e.N_SUBEM))
                : query.Count(e => !string.IsNullOrEmpty(e.N_EM) && string.IsNullOrEmpty(e.N_SUBEM));
        }

        /// <summary>
        ///     Riepilogo inviti
        /// </summary>
        /// <param name="emendamentoUId"></param>
        /// <returns></returns>
        public IEnumerable<NOTIFICHE_DESTINATARI> GetInvitati(Guid emendamentoUId)
        {
            try
            {
                var query = PRContext
                    .NOTIFICHE
                    .Where(n => n.UIDEM == emendamentoUId)
                    .Join(PRContext.NOTIFICHE_DESTINATARI,
                        n => n.UIDNotifica,
                        nd => nd.UIDNotifica,
                        (n, nd) => nd);

                return query.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     Progressivo per atto e gruppo
        /// </summary>
        /// <param name="attoUId"></param>
        /// <param name="gruppo"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public int GetProgressivo(Guid attoUId, int gruppo, bool sub)
        {
            var query = PRContext.EM
                .Where(em => em.UIDAtto == attoUId
                             && em.id_gruppo == gruppo
                             && em.Eliminato == false);
            if (sub)
                query = query.OrderByDescending(em => em.SubProgressivo)
                    .Take(1);
            else
                query = query.OrderByDescending(em => em.Progressivo)
                    .Take(1);
            var list = query.ToList();
            if (list.Count == 0)
                return 1;
            if (sub)
            {
                if (list[0].SubProgressivo.HasValue)
                    return list[0].Progressivo.Value + 1;
            }
            else
            {
                if (list[0].Progressivo.HasValue)
                    return list[0].Progressivo.Value + 1;
            }

            return 1;
        }

        /// <summary>
        ///     Ritorna tutti i valori disponibili in tabella
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PARTI_TESTO> GetPartiEmendabili()
        {
            return PRContext.PARTI_TESTO.ToList();
        }

        /// <summary>
        ///     Ritorna tutti i valori disponibili in tabella
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TIPI_EM> GetTipiEmendamento()
        {
            return PRContext.TIPI_EM.ToList();
        }

        /// <summary>
        ///     Ritorna tutti i valori disponibili in tabella
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MISSIONI> GetMissioniEmendamento()
        {
            return PRContext.MISSIONI.ToList();
        }

        /// <summary>
        ///     Ritorna tutti i valori disponibili in tabella
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TITOLI_MISSIONI> GetTitoliMissioneEmendamento()
        {
            return PRContext.TITOLI_MISSIONI.ToList();
        }

        #endregion

        #region CHECK

        /// <summary>
        ///     Controlla che l'emendamento sia eliminabile
        /// </summary>
        /// <param name="em"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        public bool CheckIfEliminabile(EmendamentiDto em, PersonaDto persona)
        {
            if (persona.Gruppo == null) return false;
            if (em.id_gruppo != persona.Gruppo.id_gruppo)
                return false;

            if (!string.IsNullOrEmpty(em.DataDeposito))
                return false;

            return persona.CurrentRole == RuoliIntEnum.Responsabile_Segreteria_Politica
                   || persona.CurrentRole == RuoliIntEnum.Responsabile_Segreteria_Giunta
                   || persona.UID_persona == em.UIDPersonaCreazione;
        }

        /// <summary>
        ///     Controlla che l'emendamento sia ritirabile
        /// </summary>
        /// <param name="em"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        public bool CheckIfRitirabile(EmendamentiDto em, PersonaDto persona)
        {
            if (persona.Gruppo == null)
                return false;

            if (em.id_gruppo != persona.Gruppo.id_gruppo)
                return false;
            if (em.DataRitiro.HasValue)
                return false;
            if (em.STATI_EM.IDStato != (int) StatiEnum.Depositato)
                return false;

            return persona.UID_persona == em.UIDPersonaProponente
                   || em.ATTI.UIDAssessoreRiferimento == persona.UID_persona
                   || persona.CurrentRole == RuoliIntEnum.Presidente_Regione
                   && em.id_gruppo >= AppSettingsConfiguration.GIUNTA_REGIONALE_ID;
        }

        /// <summary>
        ///     Controlla che l'emendamento sia depositabile
        /// </summary>
        /// <param name="em"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        public bool CheckIfDepositabile(EmendamentiDto em, PersonaDto persona)
        {
            if (!string.IsNullOrEmpty(em.DataDeposito))
                return false;

            if (persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                || persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                if (em.Firma_da_ufficio)
                    return true;

            var firmaProponente = PRContext
                .FIRME
                .Find(em.UIDEM, em.UIDPersonaProponente);

            // Se proponente non ha firmato non è possibile depositare
            if (firmaProponente == null)
                return false;
            // Se proponente ha ritirato la firma non è possibile depositare
            if (!string.IsNullOrEmpty(firmaProponente.Data_ritirofirma))
                return false;

            switch (persona.CurrentRole)
            {
                case RuoliIntEnum.Consigliere_Regionale:
                case RuoliIntEnum.Assessore_Sottosegretario_Giunta:
                case RuoliIntEnum.Presidente_Regione:
                    return em.UIDPersonaProponente == persona.UID_persona;
                case RuoliIntEnum.Amministratore_PEM:
                case RuoliIntEnum.Segreteria_Assemblea:
                    return true;
            }

            if (persona.Gruppo != null)
                return em.id_gruppo == persona.Gruppo.id_gruppo;
            return false;
        }


        /// <summary>
        ///     Controlla che l'emendamento sia modificabile dall'utente
        /// </summary>
        /// <param name="em"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        public bool CheckIfModificabile(EmendamentiDto em, PersonaDto persona)
        {
            if (string.IsNullOrEmpty(em.EM_Certificato))
                return em.UIDPersonaProponente == persona.UID_persona
                       || em.UIDPersonaCreazione == persona.UID_persona
                       || persona.CurrentRole == RuoliIntEnum.Responsabile_Segreteria_Politica
                       || persona.CurrentRole == RuoliIntEnum.Responsabile_Segreteria_Giunta;

            var counter = PRContext.FIRME.Count(f => f.UIDEM == em.UIDEM && string.IsNullOrEmpty(f.Data_ritirofirma));
            return (em.UIDPersonaProponente == persona.UID_persona || em.UIDPersonaCreazione == persona.UID_persona)
                   && (em.IDStato == (int) StatiEnum.Bozza || em.IDStato == (int) StatiEnum.Bozza_Riservata)
                   && counter == 1;
        }


        /// <summary>
        ///     Controlla che il progressivo sia unico all'interno dell'atto
        /// </summary>
        /// <param name="attoUId"></param>
        /// <param name="encrypt_progressivo"></param>
        /// <param name="counter_emendamenti"></param>
        /// <returns></returns>
        public bool CheckProgressivo(Guid attoUId, string encrypt_progressivo,
            CounterEmendamentiEnum counter_emendamenti)
        {
            var query = PRContext
                .EM
                .Where(e => true);

            switch (counter_emendamenti)
            {
                case CounterEmendamentiEnum.NONE:
                    return false;
                case CounterEmendamentiEnum.EM:
                    return !query.Any(e => e.UIDAtto == attoUId && e.N_EM == encrypt_progressivo);
                case CounterEmendamentiEnum.SUB_EM:
                    return !query.Any(e => e.UIDAtto == attoUId && e.N_SUBEM == encrypt_progressivo);
                default:
                    throw new ArgumentOutOfRangeException(nameof(counter_emendamenti), counter_emendamenti, null);
            }
        }

        #endregion

        #region SPOSTAMENTI EM IN FASE DI VOTAZIONE

        public void ORDINA_EM_TRATTAZIONE(Guid attoUId)
        {
            PRContext.Database.ExecuteSqlCommand(
                $"exec ORDINA_EM_TRATTAZIONE @UIDAtto='{attoUId}'");
        }

        public void UP_EM_TRATTAZIONE(Guid emendamentoUId)
        {
            PRContext.Database.ExecuteSqlCommand(
                $"exec UP_EM_TRATTAZIONE @UIDEM='{emendamentoUId}'");
        }

        public void DOWN_EM_TRATTAZIONE(Guid emendamentoUId)
        {
            PRContext.Database.ExecuteSqlCommand(
                $"exec DOWN_EM_TRATTAZIONE @UIDEM='{emendamentoUId}'");
        }

        public void SPOSTA_EM_TRATTAZIONE(Guid emendamentoUId, int pos)
        {
            PRContext.Database.ExecuteSqlCommand(
                $"exec SPOSTA_EM_TRATTAZIONE @UIDEM='{emendamentoUId}',@Pos={pos}");
        }

        #endregion
    }
}