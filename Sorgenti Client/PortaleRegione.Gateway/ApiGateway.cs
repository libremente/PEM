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
using System.Threading.Tasks;
using ExpressionBuilder.Common;
using ExpressionBuilder.Generics;
using Newtonsoft.Json;
using PortaleRegione.DTO.Autenticazione;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;
using PortaleRegione.DTO.Request;
using PortaleRegione.DTO.Response;
using PortaleRegione.Logger;

namespace PortaleRegione.Gateway
{
    /// <summary>
    ///     Classe per contattare gli endpoint dell'api
    /// </summary>
    public class ApiGateway : BaseGateway
    {
        #region Login

        public static async Task<LoginResponse> Login(string username, string password)
        {
            try
            {
                var requestUrl = $"{apiUrl}/autenticazione";
                var body = JsonConvert.SerializeObject(new LoginRequest
                {
                    Username = username,
                    Password = password
                });

                return JsonConvert.DeserializeObject<LoginResponse>(await Post(requestUrl, body, false));
            }
            catch (Exception ex)
            {
                Log.Error("Login", ex);
                throw ex;
            }
        }

        #endregion

        #region CambioRuolo

        public static async Task<LoginResponse> CambioRuolo(RuoliIntEnum ruolo)
        {
            try
            {
                var requestUrl = $"{apiUrl}/autenticazione?ruolo={ruolo}";

                var lst = JsonConvert.DeserializeObject<LoginResponse>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("CambioRuolo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("CambioRuolo", ex);
                throw ex;
            }
        }

        #endregion

        #region CambioGruppo

        public static async Task<LoginResponse> CambioGruppo(int gruppo)
        {
            try
            {
                var requestUrl = $"{apiUrl}/autenticazione?gruppo={gruppo}";

                var lst = JsonConvert.DeserializeObject<LoginResponse>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("CambioGruppo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("CambioGruppo", ex);
                throw ex;
            }
        }

        #endregion

        #region ### UTILS ###

        #region SendMail

        public static async Task<bool> SendMail(MailModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/util/mail";
                var body = JsonConvert.SerializeObject(model);

                return JsonConvert.DeserializeObject<bool>(await Post(requestUrl, body));
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SendMail", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SendMail", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region ### NOTIFICHE ###

        #region GetNotificheInviate

        public static async Task<BaseResponse<NotificaDto>> GetNotificheInviate(int page, int size,
            bool Archivio = false)
        {
            try
            {
                var requestUrl = $"{apiUrl}/notifiche/view-inviate";

                var model = new BaseRequest<NotificaDto>
                {
                    param = new Dictionary<string, object>(),
                    page = page,
                    size = size,
                    filtro = new List<FilterStatement<NotificaDto>>
                    {
                        new FilterStatement<NotificaDto>
                        {
                            PropertyId = nameof(NotificaDto.IDTipo),
                            Operation = Operation.EqualTo,
                            Value = (int) TipoNotificaEnum.INVITO
                        }
                    }
                };
                model.param.Add(new KeyValuePair<string, object>("Archivio", Archivio));
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<NotificaDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetNotificheInviate", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetNotificheInviate", ex);
                throw ex;
            }
        }

        #endregion

        #region GetNotificheRicevute

        public static async Task<BaseResponse<NotificaDto>> GetNotificheRicevute(int page, int size, bool Archivio)
        {
            try
            {
                var requestUrl = $"{apiUrl}/notifiche/view-ricevute";

                var model = new BaseRequest<NotificaDto>
                {
                    param = new Dictionary<string, object>(),
                    page = page,
                    size = size,
                    filtro = new List<FilterStatement<NotificaDto>>
                    {
                        new FilterStatement<NotificaDto>
                        {
                            PropertyId = nameof(NotificaDto.IDTipo),
                            Operation = Operation.EqualTo,
                            Value = (int) TipoNotificaEnum.INVITO
                        }
                    }
                };
                model.param.Add(new KeyValuePair<string, object>("Archivio", Archivio));
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<NotificaDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetNotificheRicevute", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetNotificheRicevute", ex);
                throw ex;
            }
        }

        #endregion

        #region GetDestinatariNotifica

        public static async Task<IEnumerable<DestinatariNotificaDto>> GetDestinatariNotifica(int id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/notifiche/{id}/destinatari";

                var lst = JsonConvert.DeserializeObject<IEnumerable<DestinatariNotificaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetDestinatariNotifica", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetDestinatariNotifica", ex);
                throw ex;
            }
        }

        #endregion

        #region NotificaEM

        public static async Task<Dictionary<Guid, string>> NotificaEM(ComandiAzioneModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/notifiche/invita";
                var body = JsonConvert.SerializeObject(model);
                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Post(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("NotificaEM", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("NotificaEM", ex);
                throw ex;
            }
        }

        #endregion

        #region NotificaVista

        public static async Task NotificaVista(long notificaId)
        {
            try
            {
                var requestUrl = $"{apiUrl}/notifiche/vista/{notificaId}";

                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("NotificaVista", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("NotificaVista", ex);
                throw ex;
            }
        }

        #endregion

        #region GetListaDestinatari

        public static async Task<Dictionary<string, string>> GetListaDestinatari(Guid atto,
            TipoDestinatarioNotificaEnum tipo)
        {
            try
            {
                var requestUrl = $"{apiUrl}/notifiche/destinatari?atto={atto}&tipo={(int) tipo}";
                var lst = JsonConvert.DeserializeObject<Dictionary<string, string>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetListaDestinatari", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetListaDestinatari", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region ### STAMPE ###

        #region EsportaXLS

        public static async Task<FileResponse> EsportaXLS(Guid attoUId, OrdinamentoEnum ordine)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/esporta-griglia-xls?id={attoUId}&ordine={ordine}";

                var lst = await GetFile(requestUrl);

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EsportaXLS", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EsportaXLS", ex);
                throw ex;
            }
        }

        #endregion

        #region EsportaWORD

        public static async Task<FileResponse> EsportaWORD(Guid attoUId, OrdinamentoEnum ordine)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/esporta-griglia-doc?id={attoUId}&ordine={ordine}";

                var lst = await GetFile(requestUrl);

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EsportaWORD", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EsportaWORD", ex);
                throw ex;
            }
        }

        #endregion

        #region InserisciStampa

        public static async Task InserisciStampa(BaseRequest<EmendamentiDto, StampaDto> model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/stampe";
                var body = JsonConvert.SerializeObject(model);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("InserisciStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("InserisciStampa", ex);
                throw ex;
            }
        }

        #endregion

        #region ResetStampa

        public static async Task ResetStampa(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/stampe/reset?id={id}";
                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ResetStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ResetStampa", ex);
                throw ex;
            }
        }

        #endregion

        #region GetStampe

        public static async Task<BaseResponse<StampaDto>> GetStampe(int page, int size)
        {
            try
            {
                var requestUrl = $"{apiUrl}/stampe/view";

                var model = new BaseRequest<StampaDto>
                {
                    page = page,
                    size = size
                };
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<StampaDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetStampe", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetStampe", ex);
                throw ex;
            }
        }

        #endregion

        #region JOB

        #region JobGetStampe

        public static async Task<BaseResponse<StampaDto>> JobGetStampe(int page, int size)
        {
            try
            {
                var requestUrl = $"{apiUrl}/job/stampe/view";

                var model = new BaseRequest<StampaDto>
                {
                    page = page,
                    size = size
                };
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<StampaDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("JobGetStampe", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("JobGetStampe", ex);
                throw ex;
            }
        }

        #endregion

        #region JobUnLockStampe

        public static async Task JobUnLockStampa(Guid stampaUId)
        {
            try
            {
                var requestUrl = $"{apiUrl}/job/stampe/unlock";
                var body = JsonConvert.SerializeObject(new StampaRequest
                {
                    stampaUId = stampaUId
                });
                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("JobUnLockStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("JobUnLockStampa", ex);
                throw ex;
            }
        }

        #endregion

        #region JobErrorStampa

        public static async Task JobErrorStampa(Guid stampaUId, string errorMessage)
        {
            try
            {
                var requestUrl = $"{apiUrl}/job/stampe/error";
                var body = JsonConvert.SerializeObject(new StampaRequest
                {
                    stampaUId = stampaUId,
                    messaggio = errorMessage
                });
                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("JobErrorStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("JobErrorStampa", ex);
                throw ex;
            }
        }

        #endregion

        #region JobUpdateFileStampa

        public static async Task JobUpdateFileStampa(StampaDto stampa)
        {
            try
            {
                var requestUrl = $"{apiUrl}/job/stampe";
                var body = JsonConvert.SerializeObject(stampa);
                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("JobUpdateFileStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("JobUpdateFileStampa", ex);
                throw ex;
            }
        }

        #endregion

        #region JobSetInvioStampa

        public static async Task JobSetInvioStampa(StampaDto stampa)
        {
            try
            {
                var requestUrl = $"{apiUrl}/job/stampe/inviato";
                var body = JsonConvert.SerializeObject(stampa);
                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("JobSetInvioStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("JobSetInvioStampa", ex);
                throw ex;
            }
        }

        #endregion

        #region JobGetEmendamenti

        public static async Task<BaseResponse<EmendamentiDto>> JobGetEmendamenti(string queryEM, int page,
            int size = 20)
        {
            try
            {
                var requestUrl = $"{apiUrl}/job/stampe/emendamenti";
                var body = JsonConvert.SerializeObject(new EmendamentiByQueryModel
                {
                    Query = queryEM,
                    page = page,
                    size = size
                });

                var lst = JsonConvert.DeserializeObject<BaseResponse<EmendamentiDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("JobGetEmendamenti", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("JobGetEmendamenti", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region DownloadStampa

        public static async Task<FileResponse> DownloadStampa(Guid stampaUId)
        {
            try
            {
                var requestUrl = $"{apiUrl}/stampe?id={stampaUId}";

                var lst = await GetFile(requestUrl);

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("DownloadStampa", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("DownloadStampa", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region ### EMENDAMENTI ###

        #region GetEmendamenti

        public static async Task<BaseResponse<EmendamentiDto>> GetEmendamenti(Guid attoUId, ClientModeEnum mode,
            OrdinamentoEnum ordine,
            int page, int size)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/view";

                var param = new Dictionary<string, object> {{"CLIENT_MODE", (int) mode}};

                var model = new BaseRequest<AttiDto>
                {
                    id = attoUId,
                    page = page,
                    size = size,
                    ordine = ordine,
                    param = param
                };
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<EmendamentiDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetEmendamenti", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetEmendamenti", ex);
                throw ex;
            }
        }

        #endregion

        #region GetEmendamento

        public static async Task<EmendamentiDto> GetEmendamento(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti?id={id}";

                var lst = JsonConvert.DeserializeObject<EmendamentiDto>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetEmendamento", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetEmendamento", ex);
                throw ex;
            }
        }

        #endregion

        #region GetFirmatari

        public static async Task<IEnumerable<FirmeDto>> GetFirmatari(Guid emendamentoUId, FirmeTipoEnum tipo)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/firmatari?id={emendamentoUId}&tipo={tipo}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<FirmeDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetFirmatari", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetFirmatari", ex);
                throw ex;
            }
        }

        #endregion

        #region GetInvitati

        public static async Task<IEnumerable<DestinatariNotificaDto>> GetInvitati(Guid emendamentoUId)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/invitati?id={emendamentoUId}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<DestinatariNotificaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetInvitati", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetInvitati", ex);
                throw ex;
            }
        }

        #endregion

        #region GetBodyEM

        public static async Task<string> GetBodyEM(Guid id, TemplateTypeEnum template, bool IsDeposito = false)
        {
            var result = string.Empty;
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/template-body";
                var model = new GetBodyEmendamentoModel
                {
                    Id = id,
                    Template = template,
                    IsDeposito = IsDeposito
                };
                var body = JsonConvert.SerializeObject(model);
                result = await Post(requestUrl, body);
                var lst = JsonConvert.DeserializeObject<string>(result);

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetBodyEM", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error($"GetBodyEM PARAMS: GUID EM [{id}], TEMPLATE [{template}]");
                Log.Error($"GetBodyEM RESULT: [{result}]");
                Log.Error("GetBodyEM", ex);
                throw ex;
            }
        }

        #endregion

        #region GetCopertina

        public static async Task<string> GetCopertina(CopertinaModel model)
        {
            var result = string.Empty;
            var body = string.Empty;

            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/template/copertina";
                body = JsonConvert.SerializeObject(model);

                result = await Post(requestUrl, body);
                var lst = JsonConvert.DeserializeObject<string>(result);
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetCopertina", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error($"GetCopertina PARAMS: [{body}]");
                Log.Error($"GetCopertina RESULT: [{result}]");
                Log.Error("GetCopertina", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaEM

        public static async Task EliminaEM(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/elimina?id={id}";

                JsonConvert.DeserializeObject<string>(await Get(requestUrl));
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaEM", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaEM", ex);
                throw ex;
            }
        }

        #endregion

        #region RitiraEM

        public static async Task RitiraEM(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/ritira?id={id}";

                JsonConvert.DeserializeObject<string>(await Get(requestUrl));
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("RitiraEM", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("RitiraEM", ex);
                throw ex;
            }
        }

        #endregion

        #region FirmaEM

        public static async Task<Dictionary<Guid, string>> FirmaEM(Guid emendamentoUId, string pin)
        {
            var model = new ComandiAzioneModel
            {
                ListaEmendamenti = new List<Guid> {emendamentoUId},
                Pin = pin
            };
            return await FirmaEM(model);
        }

        public static async Task<Dictionary<Guid, string>> FirmaEM(ComandiAzioneModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/firma";
                var body = JsonConvert.SerializeObject(model);
                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Post(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("FirmaEM", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("FirmaEM", ex);
                throw ex;
            }
        }

        #endregion

        #region DepositaEM

        public static async Task<Dictionary<Guid, string>> DepositaEM(Guid emendamentoUId, string pin)
        {
            var model = new ComandiAzioneModel
            {
                ListaEmendamenti = new List<Guid> {emendamentoUId},
                Pin = pin
            };
            return await DepositaEM(model);
        }

        public static async Task<Dictionary<Guid, string>> DepositaEM(ComandiAzioneModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/deposita";
                var body = JsonConvert.SerializeObject(model);
                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Post(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("DepositaEM", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("DepositaEM", ex);
                throw ex;
            }
        }

        #endregion

        #region RitiraFirma

        public static async Task<Dictionary<Guid, string>> RitiraFirma(Guid emendamentoUId, string pin)
        {
            var model = new ComandiAzioneModel
            {
                ListaEmendamenti = new List<Guid> {emendamentoUId},
                Pin = pin
            };
            return await RitiraFirma(model);
        }

        public static async Task<Dictionary<Guid, string>> RitiraFirma(ComandiAzioneModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/ritiro-firma";
                var body = JsonConvert.SerializeObject(model);
                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Post(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("RitiraFirma", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("RitiraFirma", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaFirma

        public static async Task<Dictionary<Guid, string>> EliminaFirma(Guid emendamentoUId, string pin)
        {
            var model = new ComandiAzioneModel
            {
                ListaEmendamenti = new List<Guid> {emendamentoUId},
                Pin = pin
            };
            return await EliminaFirma(model);
        }

        public static async Task<Dictionary<Guid, string>> EliminaFirma(ComandiAzioneModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/elimina-firma";
                var body = JsonConvert.SerializeObject(model);
                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Post(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaFirma", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaFirma", ex);
                throw ex;
            }
        }

        #endregion

        #region GetProgressivoTemporaneo

        public static async Task<int> GetProgressivoTemporaneo(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/progressivo?id={id}";

                var lst = JsonConvert.DeserializeObject<int>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetProgressivoTemporaneo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetProgressivoTemporaneo", ex);
                throw ex;
            }
        }

        #endregion

        #region GetNuovoEmendamentoModel

        public static async Task<EmendamentiFormModel> GetNuovoEmendamentoModel(Guid id, Guid? em_riferimentoUId)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/new?id={id}";
                if (em_riferimentoUId.HasValue)
                    requestUrl += $"&em_riferimentoUId={em_riferimentoUId}";

                var lst = JsonConvert.DeserializeObject<EmendamentiFormModel>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetNuovoEmendamentoModel", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetNuovoEmendamentoModel", ex);
                throw ex;
            }
        }

        #endregion

        #region GetModificaEmendamentoModel

        public static async Task<EmendamentiFormModel> GetModificaEmendamentoModel(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/edit?id={id}";

                var lst = JsonConvert.DeserializeObject<EmendamentiFormModel>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetModificaEmendamentoModel", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetModificaEmendamentoModel", ex);
                throw ex;
            }
        }

        #endregion

        #region GetModificaMetaDatiEmendamentoModel

        public static async Task<EmendamentiFormModel> GetModificaMetaDatiEmendamentoModel(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/edit-meta-dati?id={id}";

                var lst = JsonConvert.DeserializeObject<EmendamentiFormModel>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetModificaMetaDatiEmendamentoModel", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetModificaMetaDatiEmendamentoModel", ex);
                throw ex;
            }
        }

        #endregion

        #region SalvaEmendamento

        public static async Task SalvaEmendamento(NuovoEmendamentoRequest model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti";
                var body = JsonConvert.SerializeObject(model);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SalvaEmendamento", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SalvaEmendamento", ex);
                throw ex;
            }
        }

        #endregion

        #region ModificaEmendamento

        public static async Task ModificaEmendamento(EmendamentiDto modelEmendamento)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti";
                var body = JsonConvert.SerializeObject(modelEmendamento);

                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ModificaEmendamento", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ModificaEmendamento", ex);
                throw ex;
            }
        }

        #endregion

        #region ModificaMetaDatiEmendamento

        public static async Task ModificaMetaDatiEmendamento(EmendamentiDto modelEmendamento)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/meta-dati";
                var body = JsonConvert.SerializeObject(modelEmendamento);

                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ModificaMetaDatiEmendamento", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ModificaMetaDatiEmendamento", ex);
                throw ex;
            }
        }

        #endregion

        #region CambioStato

        public static async Task CambioStato(ModificaStatoModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/modifica-stato";
                var body = JsonConvert.SerializeObject(model);

                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("CambioStato", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("CambioStato", ex);
                throw ex;
            }
        }

        #endregion

        #region RaggruppaEmendamenti

        public static async Task<Dictionary<Guid, string>> RaggruppaEmendamenti(RaggruppaEmendamentiModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/raggruppa";
                var body = JsonConvert.SerializeObject(model);

                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Put(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("RaggruppaEmendamenti", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("RaggruppaEmendamenti", ex);
                throw ex;
            }
        }

        #endregion

        #region AssegnaNuovoPorponente

        public static async Task<Dictionary<Guid, string>> AssegnaNuovoPorponente(AssegnaProponenteModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/assegna-nuovo-proponente";
                var body = JsonConvert.SerializeObject(model);

                var result = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(await Put(requestUrl, body));

                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("AssegnaNuovoPorponente", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("AssegnaNuovoPorponente", ex);
                throw ex;
            }
        }

        #endregion

        #region ORDINAMENTI SEGRETERIA - FASCICOLO VOTAZIONE

        #region ORDINA_EM_TRATTAZIONE

        public static async Task ORDINA_EM_TRATTAZIONE(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/ordina?id={id}";
                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ORDINA_EM_TRATTAZIONE", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ORDINA_EM_TRATTAZIONE", ex);
                throw ex;
            }
        }

        #endregion

        #region UP_EM_TRATTAZIONE

        public static async Task UP_EM_TRATTAZIONE(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/ordina-up?id={id}";
                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("UP_EM_TRATTAZIONE", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("UP_EM_TRATTAZIONE", ex);
                throw ex;
            }
        }

        #endregion

        #region DOWN_EM_TRATTAZIONE

        public static async Task DOWN_EM_TRATTAZIONE(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/ordina-down?id={id}";
                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("DOWN_EM_TRATTAZIONE", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("DOWN_EM_TRATTAZIONE", ex);
                throw ex;
            }
        }

        #endregion

        #region SPOSTA_EM_TRATTAZIONE

        public static async Task SPOSTA_EM_TRATTAZIONE(Guid id, int pos)
        {
            try
            {
                var requestUrl = $"{apiUrl}/emendamenti/sposta?id={id}&pos={pos}";
                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SPOSTA_EM_TRATTAZIONE", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SPOSTA_EM_TRATTAZIONE", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #endregion

        #region ### SEDUTE ###

        #region GetSedute

        public static async Task<BaseResponse<SeduteDto>> GetSedute(int page, int size)
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute/view";

                var model = new BaseRequest<SeduteDto>
                {
                    page = page,
                    size = size
                    //,
                    //filtro = new List<FilterStatement<SeduteDto>>
                    //{
                    //    new FilterStatement<SeduteDto>
                    //    {
                    //        PropertyId = nameof(SeduteDto.UIDSeduta),
                    //        Operation = Operation.EqualTo,
                    //        Value = new Guid("481AE936-F0F8-EA11-80B7-005056904635")
                    //    }
                    //}
                };
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<SeduteDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetSedute", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetSedute", ex);
                throw ex;
            }
        }

        #endregion

        #region GetSeduta

        public static async Task<SeduteFormUpdateDto> GetSeduta(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute?id={id}";

                var lst = JsonConvert.DeserializeObject<SeduteFormUpdateDto>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetSeduta", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetSeduta", ex);
                throw ex;
            }
        }

        #endregion

        #region SalvaSeduta

        public static async Task SalvaSeduta(SeduteFormUpdateDto seduta)
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute/nuova";
                var body = JsonConvert.SerializeObject(seduta);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SalvaSeduta", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SalvaSeduta", ex);
                throw ex;
            }
        }

        #endregion

        #region ModificaSeduta

        public static async Task ModificaSeduta(SeduteFormUpdateDto seduta)
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute";
                var body = JsonConvert.SerializeObject(seduta);

                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ModificaSeduta", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ModificaSeduta", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaSeduta

        public static async Task EliminaSeduta(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute?id={id}";

                await Delete(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaSeduta", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaSeduta", ex);
                throw ex;
            }
        }

        #endregion

        #region --- FILTRI ---

        #region GetLegislature

        public static async Task<IEnumerable<LegislaturaDto>> GetLegislature()
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute/legislature";

                var lst = JsonConvert.DeserializeObject<IEnumerable<LegislaturaDto>>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetLegislature", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetLegislature", ex);
                throw ex;
            }
        }

        #endregion

        #region GetSedute

        public static async Task<BaseResponse<SeduteDto>> GetSedute(BaseRequest<SeduteDto> model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/sedute/view";
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<SeduteDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetSedute", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetSedute", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #endregion

        #region ### ATTI ###

        #region GetAtti

        public static async Task<BaseResponse<AttiDto>> GetAtti(Guid sedutaUId, ClientModeEnum mode, int page, int size)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/view";
                var param = new Dictionary<string, object> {{"CLIENT_MODE", (int) mode}};
                var model = new BaseRequest<AttiDto>
                {
                    id = sedutaUId,
                    page = page,
                    size = size,
                    param = param
                };
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<AttiDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetAtti", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetAtti", ex);
                throw ex;
            }
        }

        #endregion

        #region GetAtto

        public static async Task<AttiFormUpdateModel> GetAttoFormUpdate(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti?id={id}";

                var lst = JsonConvert.DeserializeObject<AttiFormUpdateModel>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetAttoFormUpdate", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetAttoFormUpdate", ex);
                throw ex;
            }
        }

        public static async Task<AttiDto> GetAtto(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti?id={id}";

                var lst = JsonConvert.DeserializeObject<AttiDto>(await Get(requestUrl));
                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetAtto", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetAtto", ex);
                throw ex;
            }
        }

        #endregion

        #region SalvaAtto

        public static async Task<AttiDto> SalvaAtto(AttiFormUpdateModel atto)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti";
                var body = JsonConvert.SerializeObject(atto);

                var result = JsonConvert.DeserializeObject<AttiDto>(await Post(requestUrl, body));
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SalvaAtto", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SalvaAtto", ex);
                throw ex;
            }
        }

        #endregion

        #region ModificaAtto

        public static async Task<AttiDto> ModificaAtto(AttiFormUpdateModel atto)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/modifica";
                var body = JsonConvert.SerializeObject(atto);

                var result = JsonConvert.DeserializeObject<AttiDto>(await Put(requestUrl, body));
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ModificaAtto", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ModificaAtto", ex);
                throw ex;
            }
        }

        #endregion

        #region ModificaFilesAtto

        public static async Task ModificaFilesAtto(AttiDto atto)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/fascicoli";
                var body = JsonConvert.SerializeObject(atto);

                await Put(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ModificaFilesAtto", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ModificaFilesAtto", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaAtto

        public static async Task EliminaAtto(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti?id={id}";

                await Delete(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaAtto", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaAtto", ex);
                throw ex;
            }
        }

        #endregion

        #region SalvaRelatoriAtto

        public static async Task SalvaRelatoriAtto(AttoRelatoriModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/relatori";
                var body = JsonConvert.SerializeObject(model);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SalvaRelatoriAtto", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SalvaRelatoriAtto", ex);
                throw ex;
            }
        }

        #endregion

        #region GetArticoli

        public static async Task<IEnumerable<ArticoliDto>> GetArticoli(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/articoli?id={id}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<ArticoliDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetArticoli", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetArticoli", ex);
                throw ex;
            }
        }

        #endregion

        #region CreaArticoli

        public static async Task CreaArticoli(Guid id, string articoli)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/crea-articoli?id={id}&articoli={articoli}";

                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("CreaArticoli", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("CreaArticoli", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaArticolo

        public static async Task EliminaArticolo(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/elimina-articolo?id={id}";

                await Delete(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaArticolo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaArticolo", ex);
                throw ex;
            }
        }

        #endregion

        #region GetCommi

        public static async Task<IEnumerable<CommiDto>> GetCommi(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/commi?id={id}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<CommiDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetCommi", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetCommi", ex);
                throw ex;
            }
        }

        #endregion

        #region CreaCommi

        public static async Task CreaCommi(Guid id, string commi)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/crea-commi?id={id}&commi={commi}";

                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("CreaCommi", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("CreaCommi", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaComma

        public static async Task EliminaComma(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/elimina-comma?id={id}";

                await Delete(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaComma", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaComma", ex);
                throw ex;
            }
        }

        #endregion

        #region GetLettere

        public static async Task<IEnumerable<LettereDto>> GetLettere(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/lettere?id={id}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<LettereDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetLettere", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetLettere", ex);
                throw ex;
            }
        }

        #endregion

        #region CreaLettere

        public static async Task CreaLettere(Guid id, string lettere)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/crea-lettere?id={id}&lettere={lettere}";

                await Get(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("CreaLettere", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("CreaLettere", ex);
                throw ex;
            }
        }

        #endregion

        #region EliminaLettera

        public static async Task EliminaLettera(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/elimina-lettera?id={id}";

                await Delete(requestUrl);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("EliminaLettera", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("EliminaLettera", ex);
                throw ex;
            }
        }

        #endregion

        #region PubblicaFascicolo

        public static async Task PubblicaFascicolo(PubblicaFascicoloModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/atti/abilita-fascicolazione";
                var body = JsonConvert.SerializeObject(model);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("PubblicaFascicolo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("PubblicaFascicolo", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region ### PERSONE ###

        #region GetAssessoriRiferimento

        public static async Task<IEnumerable<PersonaDto>> GetAssessoriRiferimento()
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/assessori";

                var lst = JsonConvert.DeserializeObject<IEnumerable<PersonaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetAssessoriRiferimento", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetAssessoriRiferimento", ex);
                throw ex;
            }
        }

        #endregion

        #region GetRelatori

        public static async Task<IEnumerable<PersonaDto>> GetRelatori(Guid? attoUId)
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/relatori";
                if (attoUId != Guid.Empty)
                    requestUrl += $"?id={attoUId}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<PersonaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetRelatori", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetRelatori", ex);
                throw ex;
            }
        }

        #endregion

        #region GetGruppi

        public static async Task<IEnumerable<KeyValueDto>> GetGruppi()
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/gruppi";

                var lst = JsonConvert.DeserializeObject<IEnumerable<KeyValueDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetGruppi", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetGruppi", ex);
                throw ex;
            }
        }

        #endregion

        #region GetPersona

        public static async Task<PersonaDto> GetPersona(Guid id, bool isGiunta = false)
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/{id}";
                if (isGiunta)
                    requestUrl += $"?IsGiunta={isGiunta}";

                var lst = JsonConvert.DeserializeObject<PersonaDto>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetPersona", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetPersona", ex);
                throw ex;
            }
        }

        #endregion

        #region GetPersone

        public static async Task<IEnumerable<PersonaDto>> GetPersone()
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone";
                var lst = JsonConvert.DeserializeObject<IEnumerable<PersonaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetPersone", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetPersone", ex);
                throw ex;
            }
        }

        #endregion

        #region GetRuolo

        public static async Task<RuoliDto> GetRuolo(RuoliIntEnum ruolo)
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/{(int) ruolo}";

                var lst = JsonConvert.DeserializeObject<RuoliDto>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetRuolo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetPersona", ex);
                throw ex;
            }
        }

        #endregion

        #region SalvaPin

        public static async Task SalvaPin(CambioPinModel model)
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/cambio-pin";
                var body = JsonConvert.SerializeObject(model);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("SalvaPin", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("SalvaPin", ex);
                throw ex;
            }
        }

        #endregion

        #region GetGiuntaRegionale

        public static async Task<IEnumerable<PersonaDto>> GetGiuntaRegionale()
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/giunta-regionale";

                var lst = JsonConvert.DeserializeObject<IEnumerable<PersonaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetGiuntaRegionale", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetGiuntaRegionale", ex);
                throw ex;
            }
        }

        #endregion

        #region GetSegreteriaPolitica

        public static async Task<IEnumerable<PersonaDto>> GetSegreteriaPolitica(int id, bool notifica_firma,
            bool notifica_deposito)
        {
            try
            {
                var requestUrl =
                    $"{apiUrl}/persone/gruppo/{id:int}/segreteria-politica?notifica_firma={notifica_firma}&notifica_deposito={notifica_deposito}";

                var lst = JsonConvert.DeserializeObject<IEnumerable<PersonaDto>>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetSegreteriaPolitica", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetSegreteriaPolitica", ex);
                throw ex;
            }
        }

        #endregion

        #region GetCapoGruppo

        public static async Task<PersonaDto> GetCapoGruppo(int id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/persone/gruppo/{id:int}/capo-gruppo";

                var lst = JsonConvert.DeserializeObject<PersonaDto>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetCapoGruppo", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetCapoGruppo", ex);
                throw ex;
            }
        }

        #endregion

        #endregion

        #region ### ADMIN PANEL ###

        #region GetPersonaAdmin

        public static async Task<PersonaDto> GetPersonaAdmin(Guid id)
        {
            try
            {
                var requestUrl = $"{apiUrl}/admin/view/{id}";
                var lst = JsonConvert.DeserializeObject<PersonaDto>(await Get(requestUrl));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetPersonaAdmin", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetPersonaAdmin", ex);
                throw ex;
            }
        }

        #endregion

        #region GetPersoneAdmin

        public static async Task<BaseResponse<PersonaDto>> GetPersoneAdmin(int page, int size)
        {
            try
            {
                var requestUrl = $"{apiUrl}/admin";

                var model = new BaseRequest<PersonaDto>
                {
                    page = page,
                    size = size
                };
                var body = JsonConvert.SerializeObject(model);

                var lst = JsonConvert.DeserializeObject<BaseResponse<PersonaDto>>(await Post(requestUrl, body));

                return lst;
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("GetPersoneAdmin", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("GetPersoneAdmin", ex);
                throw ex;
            }
        }

        #endregion

        #region ModificaPersona

        public static async Task ModificaPersona(PersonaDto persona)
        {
            try
            {
                var requestUrl = $"{apiUrl}/admin/salva";
                var body = JsonConvert.SerializeObject(persona);

                await Post(requestUrl, body);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error("ModificaPersona", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("ModificaPersona", ex);
                throw ex;
            }
        }

        #endregion

        #endregion
    }
}