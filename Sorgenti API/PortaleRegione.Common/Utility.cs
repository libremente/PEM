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
using System.Linq;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;

namespace PortaleRegione.Common
{
    /// <summary>
    ///     Classe con metodi in comune
    /// </summary>
    public static class Utility
    {
        #region MetaDatiEM_Label

        /// <summary>
        ///     Metodo per avere i metadati dell'emendamento in formato visualizzabile
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string MetaDatiEM_Label(EmendamentiDto em)
        {
            var result = $"Emendamento {em.TIPI_EM.Tipo_EM} - {GetParteEM(em)}";
            return result;
        }

        #endregion

        #region GetParteEM

        /// <summary>
        ///     Metodo per visualizzare la parte dell'emendamento
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetParteEM(EmendamentiDto em)
        {
            switch (em.PARTI_TESTO.IDParte)
            {
                case PartiEMEnum.Titolo_PDL:
                    return em.PARTI_TESTO.Parte;
                case PartiEMEnum.Titolo:
                    return $"Titolo: {em.NTitolo}";
                case PartiEMEnum.Capo:
                    return $"Capo: {em.NCapo}";
                case PartiEMEnum.Articolo:
                {
                    var strArticolo = string.Empty;
                    if (em.UIDArticolo.HasValue)
                        strArticolo += $"Articolo: {em.ARTICOLI.Articolo}";
                    if (em.UIDComma.HasValue && em.UIDComma.GetValueOrDefault() != Guid.Empty)
                        strArticolo += $", Comma: {em.COMMI.Comma}";
                    if (!string.IsNullOrEmpty(em.NLettera))
                    {
                        strArticolo += $", Lettera: {em.NLettera}";
                    }
                    else
                    {
                        if (em.UIDLettera.HasValue) strArticolo += $", Lettera: {em.LETTERE.Lettera}";
                    }

                    return strArticolo;
                }
                case PartiEMEnum.Missione:
                    return $"Missione: {em.NMissione} Programma: {em.NProgramma} titolo: {em.NTitoloB}";
                case PartiEMEnum.Allegato_Tabella:
                    return "Allegato/Tabella";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region EffettiFinanziariEM

        /// <summary>
        ///     Metodo per visualizzare se l'emedamento ha effetti finanziari oppure no
        /// </summary>
        /// <param name="effetti_finanziari"></param>
        /// <returns></returns>
        public static string EffettiFinanziariEM(int? effetti_finanziari)
        {
            switch (effetti_finanziari)
            {
                case 0:
                {
                    return "NO";
                }
                case 1:
                {
                    return "SI";
                }
                default:
                {
                    return "NON SPECIFICATO";
                }
            }
        }

        #endregion

        /// <summary>
        ///     Metodo per convertire un enum in KeyValueDto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<KeyValueDto> GetEnumList<T>()
        {
            return (from object e in Enum.GetValues(typeof(T))
                select new KeyValueDto
                {
                    id = (int) e,
                    descr = e.ToString().Replace("_", " ")
                }).ToList();
        }
    }
}