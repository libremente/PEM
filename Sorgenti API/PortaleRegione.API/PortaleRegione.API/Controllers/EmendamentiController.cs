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
using System.Web.Http;
using AutoMapper;
using PortaleRegione.API.Helpers;
using PortaleRegione.BAL;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;
using PortaleRegione.DTO.Request;
using PortaleRegione.DTO.Response;
using PortaleRegione.Logger;

namespace PortaleRegione.API.Controllers
{
    /// <summary>
    ///     Controller per gestire gli emendamenti
    /// </summary>
    [Authorize]
    [RoutePrefix("emendamenti")]
    public class EmendamentiController : BaseApiController
    {
        private readonly AttiLogic _logicAtti;
        private readonly EmendamentiLogic _logicEm;
        private readonly FirmeLogic _logicFirme;
        private readonly IUnitOfWork _unitOfWork;

        public EmendamentiController(PersoneLogic logicPersone, IUnitOfWork unitOfWork, AttiLogic logicAtti,
            EmendamentiLogic logicEm,
            FirmeLogic logicFirme) : base(logicPersone)
        {
            _unitOfWork = unitOfWork;
            _logicAtti = logicAtti;
            _logicEm = logicEm;
            _logicFirme = logicFirme;
        }

        /// <summary>
        ///     Endpoint per avere tutti gli emendamenti appartenenti ad un atto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("view")]
        public async Task<IHttpActionResult> GetEmendamenti(BaseRequest<EmendamentiDto> model)
        {
            ///TODO: implementare i controlli anche sull'atto

            try
            {
                var atto = _logicAtti.GetAtto(model.id);

                if (atto == null)
                    return NotFound();

                object CLIENT_MODE;
                model.param.TryGetValue("CLIENT_MODE", out CLIENT_MODE); // per trattazione aula

                var results =
                    await _logicEm.GetEmendamenti(model, SessionManager.Persona, Convert.ToInt16(CLIENT_MODE));

                return Ok(new BaseResponse<EmendamentiDto>(
                    model.page,
                    model.size,
                    results,
                    model.filtro,
                    _logicEm.CountEM(model, SessionManager.Persona, Convert.ToInt16(CLIENT_MODE)),
                    Request.RequestUri));
            }
            catch (Exception e)
            {
                Log.Error("GetEmendamenti", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere un singolo emendamento preciso
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetEmendamento(Guid id)
        {
            ///TODO: implementare i controlli anche sull'atto
            try
            {
                var em = _logicEm.GetEM(id);
                if (em == null)
                    return NotFound();

                return Ok(await _logicEm.GetEM_DTO(em, SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("GetEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint che restituisce il modello di emendamento da creare
        /// </summary>
        /// <param name="id"></param>
        /// <param name="em_riferimentoUId"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea + "," +
                           RuoliEnum.Consigliere_Regionale + "," + RuoliEnum.Assessore_Sottosegretario_Giunta + "," +
                           RuoliEnum.Responsabile_Segreteria_Politica + "," + RuoliEnum.Segreteria_Politica + "," +
                           RuoliEnum.Responsabile_Segreteria_Giunta + "," + RuoliEnum.Segreteria_Giunta_Regionale +
                           "," + RuoliEnum.Presidente_Regione)]
        [Route("new")]
        public async Task<IHttpActionResult> GetNuovoEmendamento(Guid id, Guid? em_riferimentoUId)
        {
            try
            {
                var atti = _logicAtti.GetAtto(id);
                if (atti == null)
                    return NotFound();

                return Ok(await _logicEm.ModelloNuovoEM(atti, em_riferimentoUId, SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("GetNuovoEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere l'oggetto emendamento da modificare
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea + "," +
                           RuoliEnum.Consigliere_Regionale + "," + RuoliEnum.Assessore_Sottosegretario_Giunta + "," +
                           RuoliEnum.Responsabile_Segreteria_Politica + "," + RuoliEnum.Segreteria_Politica + "," +
                           RuoliEnum.Responsabile_Segreteria_Giunta + "," + RuoliEnum.Segreteria_Giunta_Regionale +
                           "," + RuoliEnum.Presidente_Regione)]
        [Route("edit")]
        public async Task<IHttpActionResult> GetModificaEmendamento(Guid id)
        {
            try
            {
                var countFirme = _unitOfWork.Firme.CountFirme(id);
                if (countFirme > 1)
                    return BadRequest(
                        $"Non è possibile modificare l'emendamento. Ci sono ancora {countFirme} firme attive.");

                return Ok(await _logicEm.ModelloModificaEM(_logicEm.GetEM(id), SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("GetModificaEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per aggiungere un emendamento in database
        /// </summary>
        /// <param name="model">Modello emendamento da inserire</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea + "," +
                           RuoliEnum.Consigliere_Regionale + "," + RuoliEnum.Assessore_Sottosegretario_Giunta + "," +
                           RuoliEnum.Responsabile_Segreteria_Politica + "," + RuoliEnum.Segreteria_Politica + "," +
                           RuoliEnum.Responsabile_Segreteria_Giunta + "," + RuoliEnum.Segreteria_Giunta_Regionale +
                           "," + RuoliEnum.Presidente_Regione)]
        [HttpPost]
        public async Task<IHttpActionResult> NuovoEmendamento(NuovoEmendamentoRequest model)
        {
            try
            {
                if (!model.Emendamento.UIDPersonaProponente.HasValue)
                    return BadRequest("L'emendamento deve avere un proponente");
                if (model.Emendamento.IDParte == 0)
                    return BadRequest("E' obbligatorio indicare l'elemento da emendare");
                if (model.Emendamento.IDTipo_EM == 0)
                    return BadRequest("E' obbligatorio indicare il modo");
                if (string.IsNullOrEmpty(model.Emendamento.TestoEM_originale))
                    return BadRequest("Il testo dell'emendamento non può essere vuoto");
                if (model.Emendamento.IDParte == (int) PartiEMEnum.Articolo)
                    if (!model.Emendamento.UIDArticolo.HasValue)
                        return BadRequest("Manca il valore dell'articolo");
                if (model.Emendamento.IDParte == (int) PartiEMEnum.Capo)
                    if (string.IsNullOrEmpty(model.Emendamento.NCapo))
                        return BadRequest("Manca il valore del capo");
                if (model.Emendamento.IDParte == (int) PartiEMEnum.Titolo)
                    if (string.IsNullOrEmpty(model.Emendamento.NTitolo))
                        return BadRequest("Manca il valore del titolo");
                if (model.Emendamento.IDParte == (int) PartiEMEnum.Missione)
                    if (!model.Emendamento.NTitoloB.HasValue || !model.Emendamento.NMissione.HasValue ||
                        !model.Emendamento.NProgramma.HasValue)
                        return BadRequest("I valori Missione - Programma - Titolo sono obbligatori");

                var isGiunta = model.Emendamento.id_gruppo == AppSettingsConfiguration.GIUNTA_REGIONALE_ID;
                var proponente = _logicPersone.GetPersona(model.Emendamento.UIDPersonaProponente.Value, isGiunta);

                var em = await _logicEm.NuovoEmendamento(model.Emendamento, proponente, isGiunta);
                return Created(new Uri(Request.RequestUri + "/" + em.UIDEM), em);
            }
            catch (Exception e)
            {
                Log.Error("NuovoEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per modificare un emendamento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea + "," +
                           RuoliEnum.Consigliere_Regionale + "," + RuoliEnum.Assessore_Sottosegretario_Giunta + "," +
                           RuoliEnum.Responsabile_Segreteria_Politica + "," + RuoliEnum.Segreteria_Politica + "," +
                           RuoliEnum.Responsabile_Segreteria_Giunta + "," + RuoliEnum.Segreteria_Giunta_Regionale +
                           "," + RuoliEnum.Presidente_Regione)]
        [HttpPut]
        public async Task<IHttpActionResult> ModificaEmendamento(EmendamentiDto model)
        {
            try
            {
                var em = _logicEm.GetEM(model.UIDEM);

                if (em == null)
                    return NotFound();

                if (SessionManager.Persona.CurrentRole != RuoliIntEnum.Amministratore_PEM
                    && SessionManager.Persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                {
                    var countFirme = _unitOfWork.Firme.CountFirme(model.UIDEM);
                    if (countFirme > 1)
                        return BadRequest(
                            $"Non è possibile modificare l'emendamento. Ci sono ancora {countFirme} attive.");
                }

                await _logicEm.ModificaEmendamento(model, em, SessionManager.Persona);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("ModificaEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per eliminare virtualmente un emendamento
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteEmendamento(Guid id)
        {
            try
            {
                var em = _logicEm.GetEM(id);
                if (em == null)
                    return NotFound();

                var countFirme = _unitOfWork.Firme.CountFirme(id);
                if (countFirme > 0)
                    return BadRequest("L'emendamento ha delle firme attive e non può essere eliminato");

                await _logicEm.DeleteEmendamento(em, SessionManager.Persona);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("DeleteEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere i firmatari di un emendamento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        [Route("firmatari")]
        public async Task<IHttpActionResult> GetFirmatari(Guid id, FirmeTipoEnum tipo)
        {
            try
            {
                var em = _logicEm.GetEM(id);
                if (em == null)
                    return NotFound();

                var result = _logicFirme.GetFirme(em, tipo);
                var resultDto = result.Select(Mapper.Map<FIRME, FirmeDto>);

                return Ok(resultDto);
            }
            catch (Exception e)
            {
                Log.Error("GetFirmatari", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere gli invitati di un emendamento
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [Route("invitati")]
        public async Task<IHttpActionResult> GetInvitati(Guid id)
        {
            try
            {
                var em = _logicEm.GetEM(id);
                if (em == null)
                    return NotFound();

                return Ok(_logicEm.GetInvitati(em));
            }
            catch (Exception e)
            {
                Log.Error("GetInvitati", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere il corpo dell'emendamento da template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("template-body")]
        public async Task<IHttpActionResult> GetBody(GetBodyEmendamentoModel model)
        {
            try
            {
                var em = _logicEm.GetEM(model.Id);
                if (em == null)
                    return NotFound();

                var body = await _logicEm.GetBodyEM(em
                    , _logicFirme.GetFirme(em, FirmeTipoEnum.TUTTE)
                    , SessionManager.Persona
                    , model.Template
                    , model.IsDeposito);

                return Ok(body);
            }
            catch (Exception e)
            {
                Log.Error("GetBody", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per avere la copertina del fascicolo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("template/copertina")]
        public async Task<IHttpActionResult> GetBodyCopertina(CopertinaModel model)
        {
            try
            {
                var body = await _logicEm.GetCopertina(model);

                return Ok(body);
            }
            catch (Exception e)
            {
                Log.Error("GetBodyCopertina", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per firmare gli emendamenti
        /// </summary>
        /// <param name="firmaModel"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea + "," +
                           RuoliEnum.Consigliere_Regionale + "," + RuoliEnum.Assessore_Sottosegretario_Giunta + "," +
                           RuoliEnum.Presidente_Regione)]
        [HttpPost]
        [Route("firma")]
        public async Task<IHttpActionResult> FirmaEmendamento(ComandiAzioneModel firmaModel)
        {
            try
            {
                if (firmaModel.ListaEmendamenti.Count > Convert.ToInt16(AppSettingsConfiguration.LimiteFirmaMassivo))
                    return BadRequest(
                        $"Non è possibile firmare contemporaneamente più di {AppSettingsConfiguration.LimiteFirmaMassivo} emendamenti");

                var firmaUfficio = SessionManager.Persona.CurrentRole == RuoliIntEnum.Amministratore_PEM ||
                                   SessionManager.Persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea;

                if (firmaUfficio)
                {
                    if (firmaModel.Pin != AppSettingsConfiguration.MasterPIN)
                        return BadRequest("Pin inserito non valido");
                    return Ok(await _logicEm.FirmaEmendamento(firmaModel, SessionManager.Persona, null, true));
                }

                var pinInDb = _logicPersone.GetPin(SessionManager.Persona);
                if (pinInDb == null)
                    return BadRequest("Pin non impostato");
                if (pinInDb.RichiediModificaPIN)
                    return BadRequest("E' richiesto il reset del pin");
                if (firmaModel.Pin != pinInDb.PIN_Decrypt)
                    return BadRequest("Pin inserito non valido");

                return Ok(await _logicEm.FirmaEmendamento(firmaModel, SessionManager.Persona, pinInDb));
            }
            catch (Exception e)
            {
                Log.Error("FirmaEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per ritirare la firma ad un emedamento
        /// </summary>
        /// <param name="firmaModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ritiro-firma")]
        public async Task<IHttpActionResult> RitiroFirmaEmendamento(ComandiAzioneModel firmaModel)
        {
            try
            {
                var firmaUfficio = SessionManager.Persona.CurrentRole == RuoliIntEnum.Amministratore_PEM ||
                                   SessionManager.Persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea;

                if (firmaUfficio)
                {
                    if (firmaModel.Pin != AppSettingsConfiguration.MasterPIN)
                        return BadRequest("Pin inserito non valido");
                    return Ok(await _logicEm.RitiroFirmaEmendamento(firmaModel, SessionManager.Persona));
                }

                var pinInDb = _logicPersone.GetPin(SessionManager.Persona);
                if (pinInDb == null)
                    return BadRequest("Pin non impostato");
                if (pinInDb.RichiediModificaPIN)
                    return BadRequest("E' richiesto il reset del pin");
                if (firmaModel.Pin != pinInDb.PIN_Decrypt)
                    return BadRequest("Pin inserito non valido");

                return Ok(await _logicEm.RitiroFirmaEmendamento(firmaModel, SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("RitiroFirmaEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per eliminare una firma da un emendamento
        /// </summary>
        /// <param name="firmaModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("elimina-firma")]
        public async Task<IHttpActionResult> EliminaFirmaEmendamento(ComandiAzioneModel firmaModel)
        {
            try
            {
                var pinInDb = _logicPersone.GetPin(SessionManager.Persona);
                if (pinInDb == null)
                    return BadRequest("Pin non impostato");
                if (pinInDb.RichiediModificaPIN)
                    return BadRequest("E' richiesto il reset del pin");
                if (firmaModel.Pin != pinInDb.PIN_Decrypt)
                    return BadRequest("Pin inserito non valido");

                return Ok(await _logicEm.EliminaFirmaEmendamento(firmaModel, SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("EliminaFirmaEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per depositare gli emendamenti
        /// </summary>
        /// <param name="depositoModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("deposita")]
        public async Task<IHttpActionResult> DepositaEmendamento(ComandiAzioneModel depositoModel)
        {
            try
            {
                if (_logicEm.BloccaDepositi)
                    return BadRequest(
                        "E' in corso un'altra operazione di deposito. Riprova tra qualche secondo.");

                if (depositoModel.ListaEmendamenti.Count >
                    Convert.ToInt16(AppSettingsConfiguration.LimiteDepositoMassivo))
                    return BadRequest(
                        $"Non è possibile depositare contemporaneamente più di {AppSettingsConfiguration.LimiteDepositoMassivo} emendamenti");

                var depositoUfficio = SessionManager.Persona.CurrentRole == RuoliIntEnum.Amministratore_PEM ||
                                      SessionManager.Persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea;

                if (depositoUfficio)
                {
                    if (depositoModel.Pin != AppSettingsConfiguration.MasterPIN)
                        return BadRequest("Pin inserito non valido");
                    return Ok(await _logicEm.DepositaEmendamento(depositoModel, SessionManager.Persona));
                }

                var pinInDb = _logicPersone.GetPin(SessionManager.Persona);
                if (pinInDb == null)
                    return BadRequest("Pin non impostato");
                if (pinInDb.RichiediModificaPIN)
                    return BadRequest("E' richiesto il reset del pin");
                if (depositoModel.Pin != pinInDb.PIN_Decrypt)
                    return BadRequest("Pin inserito non valido");

                return Ok(await _logicEm.DepositaEmendamento(depositoModel, SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("DepositaEmendamento", e);
                return ErrorHandler(e);
            }
            finally
            {
                _logicEm.BloccaDepositi = false;
            }
        }

        /// <summary>
        ///     Endpoint per ritirare un emendamento
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ritira")]
        public async Task<IHttpActionResult> RitiraEmendamento(Guid id)
        {
            try
            {
                var em = _logicEm.GetEM(id);
                if (em == null)
                    return NotFound();

                if (DateTime.Now > em.ATTI.SEDUTE.Data_seduta)
                    return BadRequest(
                        "Non è possibile ritirare l'emendamento durante lo svolgimento della seduta: annuncia in Aula l'intenzione di ritiro");

                await _logicEm.RitiraEmendamento(em, SessionManager.Persona);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("RitiraEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per eliminare un emendamento
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("elimina")]
        public async Task<IHttpActionResult> EliminaEmendamento(Guid id)
        {
            try
            {
                var em = _logicEm.GetEM(id);
                if (em == null)
                    return NotFound();
                var firmatari = _logicFirme.GetFirme(em, FirmeTipoEnum.ATTIVI);
                var firmatari_attivi = firmatari.Where(f => string.IsNullOrEmpty(f.Data_ritirofirma));
                if (firmatari_attivi.Any())
                    return BadRequest("L'emendamento ha delle firme attive e non può essere eliminato");

                em.Eliminato = true;
                em.DataElimina = DateTime.Now;
                em.UIDPersonaElimina = SessionManager.Persona.UID_persona;

                await _unitOfWork.CompleteAsync();

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("EliminaEmendamento", e);
                return ErrorHandler(e);
            }
        }

        #region MODIFICA METADATI - SEGRETERIA

        /// <summary>
        ///     Endpoint per avere l'oggetto emendamento da modificare
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("edit-meta-dati")]
        public async Task<IHttpActionResult> GetModificaMetaDatiEmendamento(Guid id)
        {
            try
            {
                return Ok(await _logicEm.ModelloModificaEM(_logicEm.GetEM(id), SessionManager.Persona));
            }
            catch (Exception e)
            {
                Log.Error("GetModificaMetaDatiEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per modificare i metadati di un emendamento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [Route("meta-dati")]
        [HttpPut]
        public async Task<IHttpActionResult> ModificaMetaDatiEmendamento(EmendamentiDto model)
        {
            try
            {
                var em = _logicEm.GetEM(model.UIDEM);

                if (em == null)
                    return NotFound();

                await _logicEm.ModificaMetaDatiEmendamento(model, em, SessionManager.Persona);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("ModificaMetaDatiEmendamento", e);
                return ErrorHandler(e);
            }
        }

        #endregion

        #region COMANDI SEGRETERIA - GESTIONE STATI

        /// <summary>
        ///     Endpoint per modificare lo stato di una lista di emendamenti
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpPut]
        [Route("modifica-stato")]
        public async Task<IHttpActionResult> ModificaStatoEmendamento(ModificaStatoModel model)
        {
            try
            {
                return Ok(await _logicEm.ModificaStatoEmendamento(model));
            }
            catch (Exception e)
            {
                Log.Error("ModificaStatoEmendamento", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per assegnare un nuovo proponente ad una lista di emendamenti ritirati
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpPut]
        [Route("assegna-nuovo-proponente")]
        public async Task<IHttpActionResult> AssegnaNuovoPorponente(AssegnaProponenteModel model)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                foreach (var idGuid in model.ListaEmendamenti)
                {
                    var em = _logicEm.GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    if (em.STATI_EM.IDStato != (int) StatiEnum.Ritirato)
                    {
                        results.Add(idGuid,
                            $"ERROR: l'emendamento è {em.STATI_EM.Stato}, è possibile assegnare un nuovo proponente solo se lo stato è RITIRATO.");
                        continue;
                    }

                    var persona = _unitOfWork.Persone.Get(model.NuovoProponente);
                    em.IDStato = (int) StatiEnum.Depositato;
                    em.UIDPersonaProponenteOLD = em.UIDPersonaProponente;
                    em.UIDPersonaProponente = model.NuovoProponente;
                    em.id_gruppo = _logicPersone
                        .GetGruppoAttualePersona(Mapper.Map<View_UTENTI, PersonaDto>(persona), model.IsAssessore)
                        .id_gruppo;

                    await _unitOfWork.CompleteAsync();
                    results.Add(idGuid, "OK");
                }

                return Ok(results);
            }
            catch (Exception e)
            {
                Log.Error("AssegnaNuovoPorponente", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per raggruppare emendamenti assegnando un colore esadecimale
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpPut]
        [Route("raggruppa")]
        public async Task<IHttpActionResult> RaggruppaEmendamenti(RaggruppaEmendamentiModel model)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                foreach (var idGuid in model.ListaEmendamenti)
                {
                    var em = _logicEm.GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    em.Colore = model.Colore;

                    await _unitOfWork.CompleteAsync();
                    results.Add(idGuid, "OK");
                }

                return Ok(results);
            }
            catch (Exception e)
            {
                Log.Error("RaggruppaEmendamenti", e);
                return ErrorHandler(e);
            }
        }

        #endregion

        #region ORDINAMENTI SEGRETERIA - FASCICOLO VOTAZIONE

        /// <summary>
        ///     Endpoint per ordinare gli emendamenti di un atto in votazione
        /// </summary>
        /// <param name="id">Guid atto</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpGet]
        [Route("ordina")]
        public async Task<IHttpActionResult> ORDINA_EM_TRATTAZIONE(Guid id)
        {
            try
            {
                _unitOfWork.Emendamenti.ORDINA_EM_TRATTAZIONE(id);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("ORDINA_EM_TRATTAZIONE", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per spostare un emendamento di un atto in votazione in posizione superiore
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpGet]
        [Route("ordina-up")]
        public async Task<IHttpActionResult> UP_EM_TRATTAZIONE(Guid id)
        {
            try
            {
                _unitOfWork.Emendamenti.UP_EM_TRATTAZIONE(id);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("UP_EM_TRATTAZIONE", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per spostare un emendamento di un atto in votazione in posizione inferiore
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpGet]
        [Route("ordina-down")]
        public async Task<IHttpActionResult> DOWN_EM_TRATTAZIONE(Guid id)
        {
            try
            {
                _unitOfWork.Emendamenti.DOWN_EM_TRATTAZIONE(id);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("DOWN_EM_TRATTAZIONE", e);
                return ErrorHandler(e);
            }
        }

        /// <summary>
        ///     Endpoint per spostare un emendamento di un atto in votazione in una posizione precisa
        /// </summary>
        /// <param name="id">Guid emendamento</param>
        /// <param name="pos">Int posizione dove spostare l'emendamento</param>
        /// <returns></returns>
        [Authorize(Roles = RuoliEnum.Amministratore_PEM + "," + RuoliEnum.Segreteria_Assemblea)]
        [HttpGet]
        [Route("sposta")]
        public async Task<IHttpActionResult> SPOSTA_EM_TRATTAZIONE(Guid id, int pos)
        {
            try
            {
                _unitOfWork.Emendamenti.SPOSTA_EM_TRATTAZIONE(id, pos);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error("SPOSTA_EM_TRATTAZIONE", e);
                return ErrorHandler(e);
            }
        }

        #endregion
    }
}