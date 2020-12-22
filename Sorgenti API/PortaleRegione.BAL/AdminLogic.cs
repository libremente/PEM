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
using AutoMapper;
using ExpressionBuilder.Generics;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Request;
using PortaleRegione.Logger;

namespace PortaleRegione.BAL
{
    public class AdminLogic : BaseLogic
    {
        private readonly PersoneLogic _logicPersona;
        private readonly IUnitOfWork _unitOfWork;

        public AdminLogic(IUnitOfWork unitOfWork, PersoneLogic logicPersona)
        {
            _unitOfWork = unitOfWork;
            _logicPersona = logicPersona;
        }

        #region GetPersoneIn_DB

        public IEnumerable<PersonaDto> GetPersoneIn_DB(BaseRequest<PersonaDto> model)
        {
            try
            {
                var queryFilter = new Filter<View_UTENTI>();
                queryFilter.ImportStatements(model.filtro);

                return _unitOfWork
                    .Persone
                    .GetAll(model.page,
                        model.size,
                        queryFilter)
                    .ToList()
                    .Select(Mapper.Map<View_UTENTI, PersonaDto>);
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetPersoneIn_DB", e);
                throw e;
            }
        }

        #endregion

        #region GetPersona

        public PersonaDto GetPersona(int id)
        {
            return _logicPersona.GetPersona(id);
        }

        #endregion

        #region Count

        public int Count(BaseRequest<PersonaDto> model)
        {
            try
            {
                var queryFilter = new Filter<View_UTENTI>();
                queryFilter.ImportStatements(model.filtro);

                return _unitOfWork
                    .Persone
                    .CountAll(queryFilter);
            }
            catch (Exception e)
            {
                Log.Error("Logic - CountAll", e);
                throw e;
            }
        }

        #endregion
    }
}