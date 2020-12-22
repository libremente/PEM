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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class StampeLogic : BaseLogic
    {
        #region ctor

        public StampeLogic(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region GetStampa

        public STAMPE GetStampa(Guid id)
        {
            try
            {
                var result = _unitOfWork.Stampe.Get(id);
                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetStampa", e);
                throw e;
            }
        }

        #endregion

        #region InserisciStampa

        public async Task InserisciStampa(BaseRequest<EmendamentiDto, StampaDto> model, PersonaDto persona)
        {
            try
            {
                var queryFilter = new Filter<EM>();
                queryFilter.ImportStatements(model.filtro);

                var queryEM =
                    _unitOfWork.Emendamenti.GetAll_Query(model.entity.UIDAtto, persona, model.ordine, queryFilter);
                var stampa = Mapper.Map<StampaDto, STAMPE>(model.entity);
                stampa.QueryEM = queryEM;

                stampa.DataRichiesta = DateTime.Now;
                stampa.CurrentRole = (int) persona.CurrentRole;
                stampa.UIDStampa = Guid.NewGuid();
                stampa.UIDUtenteRichiesta = persona.UID_persona;
                stampa.Lock = false;
                stampa.Tentativi = 0;
                if (stampa.A == 0 && stampa.Da == 0)
                    stampa.Scadenza = null;
                else
                    stampa.Scadenza =
                        DateTime.Now.AddDays(Convert.ToDouble(AppSettingsConfiguration.GiorniValiditaLink));

                _unitOfWork.Stampe.Add(stampa);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - InserisciStampa", e);
                throw;
            }
        }

        #endregion

        #region LockStampa

        public async Task LockStampa(IEnumerable<StampaDto> listaStampe)
        {
            try
            {
                foreach (var stampa in listaStampe.Select(stampaDto => _unitOfWork.Stampe.Get(stampaDto.UIDStampa)))
                {
                    stampa.Lock = true;
                    stampa.DataLock = DateTime.Now;
                    stampa.DataInizioEsecuzione = DateTime.Now;
                    stampa.Tentativi += 1;

                    await _unitOfWork.CompleteAsync();
                }
            }
            catch (Exception e)
            {
                Log.Error("Logic - LockStampa", e);
                throw;
            }
        }

        #endregion

        #region UnLockStampa

        public async Task UnLockStampa(Guid stampaUId)
        {
            try
            {
                var stampa = _unitOfWork.Stampe.Get(stampaUId);
                stampa.Lock = false;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - UnLockStampa", e);
                throw;
            }
        }

        #endregion

        #region ResetStampa

        public async Task ResetStampa(STAMPE stampa)
        {
            try
            {
                stampa.Lock = false;
                stampa.Tentativi = 0;
                stampa.DataLock = null;
                stampa.DataInizioEsecuzione = null;
                stampa.MessaggioErrore = string.Empty;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - ResetStampa", e);
                throw e;
            }
        }

        #endregion

        #region ErroreStampa

        public async Task ErroreStampa(StampaRequest model)
        {
            try
            {
                var stampa = _unitOfWork.Stampe.Get(model.stampaUId);
                stampa.DataFineEsecuzione = DateTime.Now;
                stampa.MessaggioErrore = model.messaggio;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - ErroreStampa", e);
                throw;
            }
        }

        #endregion

        #region UpdateFileStampa

        public async Task UpdateFileStampa(StampaDto stampa)
        {
            try
            {
                var stampaInDb = _unitOfWork.Stampe.Get(stampa.UIDStampa);
                stampaInDb.DataFineEsecuzione = DateTime.Now;
                stampaInDb.PathFile = stampa.PathFile;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - UpdateFileStampa", e);
                throw;
            }
        }

        #endregion

        #region SetInvioStampa

        public async Task SetInvioStampa(StampaDto stampa)
        {
            try
            {
                var stampaInDb = _unitOfWork.Stampe.Get(stampa.UIDStampa);
                stampaInDb.Invio = true;
                stampaInDb.DataInvio = DateTime.Now;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - SetInvioStampa", e);
                throw;
            }
        }

        #endregion

        #region DownloadStampa

        public async Task<HttpResponseMessage> DownloadStampa(STAMPE stampa)
        {
            try
            {
                var _pathTemp = string.Empty;
                _pathTemp = stampa.NotificaDepositoEM
                    ? AppSettingsConfiguration.RootRepository
                    : AppSettingsConfiguration.CartellaLavoroStampe;

                var stream = new MemoryStream();
                using (var fileStream = new FileStream(Path.Combine(_pathTemp, stampa.PathFile), FileMode.Open))
                {
                    await fileStream.CopyToAsync(stream);
                }

                stream.Position = 0;
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.GetBuffer())
                };
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(stampa.PathFile)
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - DownloadStampa", e);
                throw e;
            }
        }

        #endregion

        #region GetStampe

        public async Task<BaseResponse<StampaDto>> GetStampe(BaseRequest<StampaDto> model, PersonaDto persona, Uri url)
        {
            try
            {
                Log.Debug($"Logic - GetStampe - page[{model.page}], pageSize[{model.size}]");
                var queryFilter = new Filter<STAMPE>();
                queryFilter.ImportStatements(model.filtro);

                var result = _unitOfWork.Stampe.GetAll(persona, model.page, model.size, queryFilter)
                    .Select(Mapper.Map<STAMPE, StampaDto>);
                return new BaseResponse<StampaDto>(
                    model.page,
                    model.size,
                    result,
                    model.filtro,
                    _unitOfWork.Stampe.Count(persona, queryFilter),
                    url);
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetStampe", e);
                throw e;
            }
        }

        public async Task<BaseResponse<StampaDto>> GetStampe(BaseRequest<StampaDto> model, Uri url)
        {
            try
            {
                Log.Debug($"Logic - GetStampe - page[{model.page}], pageSize[{model.size}]");

                var result = _unitOfWork.Stampe.GetAll(model.page, model.size)
                    .Select(Mapper.Map<STAMPE, StampaDto>);

                await LockStampa(result);

                return new BaseResponse<StampaDto>(
                    model.page,
                    model.size,
                    result,
                    model.filtro,
                    _unitOfWork.Stampe.Count(),
                    url);
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetStampe", e);
                throw e;
            }
        }

        #endregion
    }
}