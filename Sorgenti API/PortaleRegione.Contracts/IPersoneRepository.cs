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
using ExpressionBuilder.Generics;
using PortaleRegione.Domain;

namespace PortaleRegione.Contracts
{
    /// <summary>
    ///     Interfaccia Persone
    /// </summary>
    public interface IPersoneRepository : IRepository<View_UTENTI>
    {
        View_UTENTI Get(string login_windows);
        View_UTENTI Get(Guid personaUId);
        View_UTENTI Get(int personaId);
        IEnumerable<View_UTENTI> GetAll(int page, int size, Filter<View_UTENTI> filtro = null);
        int CountAll(Filter<View_UTENTI> filtro = null);
        IEnumerable<View_UTENTI> GetAll_DA_CANCELLARE();
        IEnumerable<View_UTENTI> GetAssessoriRiferimento(int id_legislatura);
        IEnumerable<View_UTENTI> GetRelatori(Guid? attoUId);
        bool IsRelatore(Guid personaUId, Guid attoUId);
        bool IsAssessore(Guid personaUId, Guid attoUId);
        string GetCarica(Guid personaUId);
        View_PINS GetPin(Guid personaUId);

        IEnumerable<View_UTENTI> GetConsiglieri(int id_legislatura);

        IEnumerable<View_Composizione_GiuntaRegionale> GetGiuntaRegionale();

        IEnumerable<UTENTI_NoCons> GetSegreteriaGiuntaRegionale(int id, bool notifica_firma,
            bool notifica_deposito);

        void SavePin(Guid personaUId, string nuovo_pin, bool reset);
    }
}