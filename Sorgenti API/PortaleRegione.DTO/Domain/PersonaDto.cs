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
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;

namespace PortaleRegione.DTO.Domain
{
    public class PersonaDto
    {
        public string DisplayName => $"{cognome.Replace("'", "&rsquo;")} {nome.Replace("'", "&rsquo;")}";

        public string DisplayName_GruppoCode =>
            $"{DisplayName} ({(Gruppo != null ? Gruppo.codice_gruppo : "N.D.")})";

        public string DisplayName_GruppoCode_EX =>
            $"{DisplayName} ({(Gruppo != null ? Gruppo.nome_gruppo : "N.D.")})";

        public Guid UID_persona { get; set; }
        public int id_persona { get; set; }

        public string cognome { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string foto { get; set; }
        public string userAD { get; set; }
        public int No_Cons { get; set; }

        public string Carica { get; set; }

        public IEnumerable<RuoliDto> Ruoli { get; set; }
        public GruppiDto Gruppo { get; set; }
        public RuoliIntEnum CurrentRole { get; set; }
        public IEnumerable<KeyValueDto> GruppiAdmin { get; set; }


        public bool IsGiunta()
        {
            switch (CurrentRole)
            {
                case RuoliIntEnum.Presidente_Regione:
                case RuoliIntEnum.Assessore_Sottosegretario_Giunta:
                case RuoliIntEnum.Amministratore_Giunta:
                case RuoliIntEnum.Responsabile_Segreteria_Giunta:
                case RuoliIntEnum.Segreteria_Giunta_Regionale:
                    return true;
                default:
                    return false;
            }
        }
    }
}