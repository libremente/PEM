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
using System.Threading.Tasks;
using AutoMapper;
using ExpressionBuilder.Generics;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Request;
using PortaleRegione.DTO.Response;
using PortaleRegione.Logger;

namespace PortaleRegione.BAL
{
    public class SeduteLogic : BaseLogic
    {
        #region ctor

        public SeduteLogic(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region GetSedute

        public BaseResponse<SeduteDto> GetSedute(BaseRequest<SeduteDto> model, Uri url)
        {
            try
            {
                Log.Debug($"Logic - GetSedute - page[{model.page}], pageSize[{model.size}]");
                var queryFilter = new Filter<SEDUTE>();
                queryFilter.ImportStatements(model.filtro);

                var result = _unitOfWork.Sedute
                    .GetAll(_unitOfWork.Legislature.Legislatura_Attiva(), model.page, model.size, queryFilter)
                    .Select(Mapper.Map<SEDUTE, SeduteDto>);
                return new BaseResponse<SeduteDto>(
                    model.page,
                    model.size,
                    result,
                    model.filtro,
                    _unitOfWork.Sedute.Count(_unitOfWork.Legislature.Legislatura_Attiva(), queryFilter),
                    url);
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetSedute", e);
                throw e;
            }
        }

        #endregion

        #region GetSeduta

        public SEDUTE GetSeduta(Guid id)
        {
            try
            {
                var result = _unitOfWork.Sedute.Get(id);
                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetSeduta", e);
                throw e;
            }
        }

        #endregion

        #region DeleteSeduta

        public async Task DeleteSeduta(SeduteDto sedutaDto, PersonaDto persona)
        {
            try
            {
                var sedutaInDb = _unitOfWork.Sedute.Get(sedutaDto.UIDSeduta);
                sedutaInDb.Eliminato = true;
                sedutaInDb.UIDPersonaModifica = persona.UID_persona;
                sedutaInDb.DataModifica = DateTime.Now;
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - DeleteSeduta", e);
                throw e;
            }
        }

        #endregion

        #region NuovaSeduta

        public async Task<SEDUTE> NuovaSeduta(SEDUTE seduta, PersonaDto persona)
        {
            try
            {
                seduta.UIDSeduta = Guid.NewGuid();
                seduta.Eliminato = false;
                seduta.UIDPersonaCreazione = persona.UID_persona;
                seduta.DataCreazione = DateTime.Now;
                seduta.id_legislatura = _unitOfWork.Legislature.Legislatura_Attiva();
                _unitOfWork.Sedute.Add(seduta);
                await _unitOfWork.CompleteAsync();
                return GetSeduta(seduta.UIDSeduta);
            }
            catch (Exception e)
            {
                Log.Error("Logic - NuovaSeduta", e);
                throw e;
            }
        }

        #endregion

        #region ModificaSeduta

        public async Task ModificaSeduta(SeduteFormUpdateDto sedutaDto, PersonaDto persona)
        {
            try
            {
                var sedutaInDb = _unitOfWork.Sedute.Get(sedutaDto.UIDSeduta);
                sedutaInDb.UIDPersonaModifica = persona.UID_persona;
                sedutaInDb.DataModifica = DateTime.Now;
                Mapper.Map(sedutaDto, sedutaInDb);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - ModificaSeduta", e);
                throw e;
            }
        }

        #endregion

        #region ### FILTRI ###

        public IEnumerable<LegislaturaDto> GetLegislature()
        {
            return _unitOfWork.Legislature.GetLegislature().Select(Mapper.Map<legislature, LegislaturaDto>);
        }

        #endregion
    }
}