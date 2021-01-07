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
using PortaleRegione.BAL.OpenData;
using PortaleRegione.Common;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Domain.Essentials;
using PortaleRegione.DTO.Enum;
using PortaleRegione.DTO.Model;
using PortaleRegione.DTO.Request;
using PortaleRegione.Logger;

namespace PortaleRegione.BAL
{
    public class EmendamentiLogic : BaseLogic
    {
        private readonly FirmeLogic _logicFirme;
        private readonly PersoneLogic _logicPersone;
        private readonly UtilsLogic _logicUtil;

        #region ctor

        public EmendamentiLogic(IUnitOfWork unitOfWork, FirmeLogic logicFirme, PersoneLogic logicPersone,
            UtilsLogic logicUtil)
        {
            _logicFirme = logicFirme;
            _logicPersone = logicPersone;
            _logicUtil = logicUtil;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public bool BloccaDepositi { get; set; }

        #region ModelloNuovoEM

        public async Task<EmendamentiFormModel> ModelloNuovoEM(ATTI atto, Guid? em_riferimentoUId, PersonaDto persona)
        {
            try
            {
                var sub_em = em_riferimentoUId != Guid.Empty;

                var result = new EmendamentiFormModel();
                var emendamento = new EmendamentiDto();

                var isGiunta = persona.IsGiunta();

                var progressivo = persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                                  || persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea
                    ? 1
                    : _unitOfWork.Emendamenti.GetProgressivo(atto.UIDAtto,
                        persona.Gruppo.id_gruppo, sub_em);

                if (sub_em)
                {
                    emendamento.SubProgressivo = progressivo;
                    emendamento.Rif_UIDEM = em_riferimentoUId;
                    emendamento.IDStato = (int) StatiEnum.Bozza;
                }
                else
                {
                    emendamento.Progressivo = progressivo;
                    if (persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                        || persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea
                        || isGiunta)
                        emendamento.IDStato = (int) StatiEnum.Bozza;
                    else
                        emendamento.IDStato = persona.Gruppo.abilita_em_privati
                            ? (int) StatiEnum.Bozza_Riservata
                            : (int) StatiEnum.Bozza;
                }

                emendamento.DisplayTitle = GetNomeEM(emendamento,
                    emendamento.Rif_UIDEM.HasValue
                        ? Mapper.Map<EM, EmendamentiDto>(GetEM(emendamento.Rif_UIDEM.Value))
                        : null);

                if (persona.CurrentRole == RuoliIntEnum.Consigliere_Regionale ||
                    persona.CurrentRole == RuoliIntEnum.Assessore_Sottosegretario_Giunta)
                {
                    emendamento.UIDPersonaProponente = persona.UID_persona;
                    emendamento.PersonaProponente = new PersonaLightDto
                    {
                        UID_persona = persona.UID_persona,
                        cognome = persona.cognome,
                        nome = persona.nome
                    };
                }
                else
                {
                    if (persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                        || persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                    {
                        result.ListaConsiglieri =
                            _logicPersone.GetConsiglieri();
                        result.ListaAssessori = _logicPersone
                            .GetAssessoriRiferimento();
                    }
                    else
                    {
                        if (isGiunta)
                            result.ListaGruppo = _logicPersone
                                .GetAssessoriRiferimento();
                        else
                            result.ListaGruppo = _logicPersone.GetConsiglieriGruppo(persona.Gruppo.id_gruppo);
                    }
                }

                emendamento.UIDPersonaCreazione = persona.UID_persona;
                emendamento.DataCreazione = DateTime.Now;
                emendamento.idRuoloCreazione = (int) persona.CurrentRole;
                //if (isGiunta)
                //    emendamento.id_gruppo = AppSettingsConfiguration.GIUNTA_REGIONALE_ID;
                //else if (persona.CurrentRole != RuoliIntEnum.Amministratore_PEM
                //         && persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                //    emendamento.id_gruppo = persona.Gruppo.id_gruppo;
                if (persona.CurrentRole != RuoliIntEnum.Amministratore_PEM
                    && persona.CurrentRole != RuoliIntEnum.Segreteria_Assemblea)
                    emendamento.id_gruppo = persona.Gruppo.id_gruppo;
                emendamento.UIDAtto = atto.UIDAtto;
                emendamento.ATTI = Mapper.Map<ATTI, AttiDto>(atto);

                result.ListaPartiEmendabili = _unitOfWork
                    .Emendamenti
                    .GetPartiEmendabili()
                    .Select(Mapper.Map<PARTI_TESTO, PartiTestoDto>);
                result.ListaTipiEmendamento = _unitOfWork
                    .Emendamenti
                    .GetTipiEmendamento()
                    .Select(Mapper.Map<TIPI_EM, Tipi_EmendamentiDto>);
                result.ListaMissioni = _unitOfWork
                    .Emendamenti
                    .GetMissioniEmendamento()
                    .Select(Mapper.Map<MISSIONI, MissioniDto>);
                result.ListaTitoli_Missioni = _unitOfWork
                    .Emendamenti
                    .GetTitoliMissioneEmendamento()
                    .Select(Mapper.Map<TITOLI_MISSIONI, TitoloMissioniDto>);
                result.ListaArticoli = _unitOfWork
                    .Articoli
                    .GetArticoli(atto.UIDAtto)
                    .Select(Mapper.Map<ARTICOLI, ArticoliDto>);

                result.Emendamento = emendamento;

                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - ModelloNuovoEM", e);
                throw e;
            }
        }

        #endregion

        #region ModelloModificaEM

        public async Task<EmendamentiFormModel> ModelloModificaEM(EM emInDb, PersonaDto persona)
        {
            try
            {
                var em = await GetEM_DTO(emInDb, persona);

                var result = new EmendamentiFormModel {Emendamento = em};
                if (persona.CurrentRole != RuoliIntEnum.Consigliere_Regionale &&
                    persona.CurrentRole != RuoliIntEnum.Assessore_Sottosegretario_Giunta)
                {
                    if (persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                        || persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                    {
                        result.ListaConsiglieri =
                            _unitOfWork.Persone.GetConsiglieri(_unitOfWork.Legislature.Legislatura_Attiva())
                                .Select(Mapper.Map<View_UTENTI, PersonaDto>);
                        result.ListaAssessori = _unitOfWork.Persone
                            .GetAssessoriRiferimento(_unitOfWork.Legislature.Legislatura_Attiva())
                            .Select(Mapper.Map<View_UTENTI, PersonaDto>);
                        result.ListaAreaPolitica = Utility.GetEnumList<AreaPoliticaIntEnum>();
                    }
                    else
                    {
                        result.ListaGruppo = _unitOfWork.Gruppi.GetConsiglieriGruppo(
                                _unitOfWork.Legislature.Legislatura_Attiva(), persona.Gruppo.id_gruppo)
                            .Select(Mapper.Map<View_UTENTI, PersonaDto>);
                    }
                }

                result.ListaPartiEmendabili = _unitOfWork
                    .Emendamenti
                    .GetPartiEmendabili()
                    .Select(Mapper.Map<PARTI_TESTO, PartiTestoDto>);
                result.ListaTipiEmendamento = _unitOfWork
                    .Emendamenti
                    .GetTipiEmendamento()
                    .Select(Mapper.Map<TIPI_EM, Tipi_EmendamentiDto>);
                result.ListaMissioni = _unitOfWork
                    .Emendamenti
                    .GetMissioniEmendamento()
                    .Select(Mapper.Map<MISSIONI, MissioniDto>);
                result.ListaTitoli_Missioni = _unitOfWork
                    .Emendamenti
                    .GetTitoliMissioneEmendamento()
                    .Select(Mapper.Map<TITOLI_MISSIONI, TitoloMissioniDto>);
                result.ListaArticoli = _unitOfWork
                    .Articoli
                    .GetArticoli(em.UIDAtto)
                    .Select(Mapper.Map<ARTICOLI, ArticoliDto>);

                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - ModelloModificaEM", e);
                throw e;
            }
        }

        #endregion

        #region NuovoEmendamento

        public async Task<EM> NuovoEmendamento(EmendamentiDto emendamentoDto, PersonaDto persona, bool isGiunta = false)
        {
            try
            {
                var progressivo =
                    _unitOfWork.Emendamenti.GetProgressivo(emendamentoDto.UIDAtto,
                        persona.Gruppo.id_gruppo,
                        emendamentoDto.Rif_UIDEM.HasValue);
                if (emendamentoDto.Rif_UIDEM.HasValue)
                    emendamentoDto.SubProgressivo = progressivo;
                else
                    emendamentoDto.Progressivo = progressivo;

                var em = Mapper.Map<EmendamentiDto, EM>(emendamentoDto);
                em.UIDEM = Guid.NewGuid();
                em.UID_QRCode = Guid.NewGuid();
                em.Eliminato = false;
                em.DataCreazione = DateTime.Now;
                em.OrdinePresentazione = 1;
                em.id_gruppo = persona.Gruppo.id_gruppo;
                _unitOfWork.Emendamenti.Add(em);
                await _unitOfWork.CompleteAsync();
                return em;
            }
            catch (Exception e)
            {
                Log.Error("Logic - NuovoEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region ModificaEmendamento

        public async Task ModificaEmendamento(EmendamentiDto model, EM em, PersonaDto persona)
        {
            try
            {
                var updateDto = Mapper.Map<EmendamentiDto, EmendamentoLightDto>(model);
                Mapper.Map(updateDto, em);

                var countFirme = _unitOfWork.Firme.CountFirme(model.UIDEM);

                if (!string.IsNullOrEmpty(em.EM_Certificato) && countFirme == 1)
                {
                    //cancelliamo firme - notifiche - stampe
                    em.UIDPersonaPrimaFirma = Guid.Empty;
                    em.DataPrimaFirma = null;
                    em.EM_Certificato = string.Empty;
                    em.Hash = string.Empty;
                    _unitOfWork.Firme.CancellaFirme(model.UIDEM);

                    ///TODO: Cancellare notifiche
                }

                em.UIDPersonaModifica = persona.UID_persona;
                em.DataModifica = DateTime.Now;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - ModificaEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region ModificaMetaDatiEmendamento

        public async Task ModificaMetaDatiEmendamento(EmendamentiDto model, EM em, PersonaDto persona)
        {
            try
            {
                if (model.TestoEM_Modificabile == em.TestoEM_originale) model.TestoEM_Modificabile = string.Empty;

                var updateMetaDatiDto = Mapper.Map<EmendamentiDto, MetaDatiEMDto>(model);
                var emAggiornato = Mapper.Map(updateMetaDatiDto, em);

                emAggiornato.UIDPersonaModifica = persona.UID_persona;
                emAggiornato.DataModifica = DateTime.Now;

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - ModificaMetaDatiEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region DeleteEmendamento

        public async Task DeleteEmendamento(EM em, PersonaDto persona)
        {
            try
            {
                em.Eliminato = true;
                em.UIDPersonaModifica = persona.UID_persona;
                em.DataModifica = DateTime.Now;
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - DeleteEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region GetInvitati

        public IEnumerable<DestinatariNotificaDto> GetInvitati(EM em)
        {
            try
            {
                var result = _unitOfWork
                    .Emendamenti
                    .GetInvitati(em.UIDEM)
                    .Select(Mapper.Map<NOTIFICHE_DESTINATARI, DestinatariNotificaDto>)
                    .ToList();

                result.ForEach(destinatario =>
                {
                    destinatario.Firmato = _unitOfWork
                        .Firme
                        .CheckFirmato(destinatario.NOTIFICHE.UIDEM, destinatario.UIDPersona);
                });
                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetInvitati", e);
                throw e;
            }
        }

        #endregion

        #region GetBodyEM

        public async Task<string> GetBodyEM(EM em, IEnumerable<FIRME> firme, PersonaDto persona,
            TemplateTypeEnum template, bool isDeposito = false)
        {
            try
            {
                var emendamentoDto = await GetEM_DTO(em, persona);
                var firmeDto = firme.Select(Mapper.Map<FIRME, FirmeDto>);

                try
                {
                    var body = GetTemplate(template);

                    switch (template)
                    {
                        case TemplateTypeEnum.MAIL:
                            GetBodyMail(emendamentoDto, firmeDto, isDeposito, ref body);
                            break;
                        case TemplateTypeEnum.PDF:
                            GetBodyPDF(emendamentoDto, firmeDto, persona, ref body);
                            break;
                        case TemplateTypeEnum.HTML:
                            GetBodyTemporaneo(emendamentoDto, ref body);
                            break;
                        case TemplateTypeEnum.HTML_MODIFICABILE:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(template), template, null);
                    }

                    return body;
                }
                catch (Exception e)
                {
                    Log.Error("GetBodyEM", e);
                    throw e;
                }
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetBodyEM", e);
                throw e;
            }
        }

        #endregion

        #region GetBodyCopertina

        public async Task<string> GetCopertina(CopertinaModel model)
        {
            try
            {
                var countEM_Atto = await _unitOfWork.Atti.CountEM(model.Atto.UIDAtto, false, null, 0);
                var countSUBEM_Atto = await _unitOfWork.Atti.CountEM(model.Atto.UIDAtto, true, null, 0);

                var body = GetTemplate(TemplateTypeEnum.PDF_COPERTINA);
                body = body.Replace("{NumeroProgettoLegge}", model.Atto.NAtto);
                body = body.Replace("{OggettoProgettoLegge}", model.Atto.Oggetto);
                body = body.Replace("{DataOdierna}", DateTime.Now.ToString("dd/MM/yyyy"));

                if (countEM_Atto + countSUBEM_Atto != model.TotaleEM)
                {
                    body = body.Replace("{CountEM}", $"EM/SUBEM estratti: {model.TotaleEM}");
                    body = body.Replace("{CountSUBEM}", string.Empty);
                }
                else
                {
                    body = body.Replace("{CountEM}", $"EM: {countEM_Atto}");
                    body = body.Replace("{CountSUBEM}", $"/ SUBEM: {countSUBEM_Atto}");
                }

                body = body.Replace("{ORDINE}",
                    model.Ordinamento == OrdinamentoEnum.Votazione ? "VOTAZIONE" : "PRESENTAZIONE");

                return body;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetCopertina", e);
                throw e;
            }
        }

        #endregion

        #region FirmaEmendamento

        public async Task<Dictionary<Guid, string>> FirmaEmendamento(ComandiAzioneModel firmaModel, PersonaDto persona,
            PinDto pin, bool firmaUfficio = false)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                foreach (var idGuid in firmaModel.ListaEmendamenti)
                {
                    var em = GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    var n_em = GetNomeEM(Mapper.Map<EM, EmendamentiDto>(em),
                        em.Rif_UIDEM.HasValue
                            ? Mapper.Map<EM, EmendamentiDto>(GetEM(em.Rif_UIDEM.Value))
                            : null);

                    if (em.STATI_EM.IDStato > (int) StatiEnum.Depositato)
                        results.Add(idGuid, $"ERROR: Emendamento {n_em} già votato e non è più sottoscrivibile");

                    var firmaCert = string.Empty;

                    if (firmaUfficio)
                    {
                        var firmato_ufficio = _unitOfWork.Firme.CheckFirmatoDaUfficio(idGuid);

                        //Controllo se l'utente ha già firmato
                        if (firmato_ufficio)
                        {
                            results.Add(idGuid, $"ERROR: Emendamento {n_em} già firmato dall'ufficio");
                            continue;
                        }

                        firmaCert = EncryptString($"Inserito d'ufficio ({AppSettingsConfiguration.UtenteFirmaUfficio})"
                            , AppSettingsConfiguration.masterKey);
                    }
                    else
                    {
                        var firmato_utente = _unitOfWork.Firme.CheckFirmato(idGuid, persona.UID_persona);

                        //Controllo se l'utente ha già firmato
                        if (firmato_utente)
                        {
                            results.Add(idGuid, $"ERROR: Emendamento {n_em} già firmato");
                            continue;
                        }

                        var firmato_dal_proponente = em.STATI_EM.IDStato >= (int) StatiEnum.Depositato
                            ? true
                            : _unitOfWork.Firme.CheckFirmato(em.UIDEM, em.UIDPersonaProponente.Value);

                        //Controllo la firma del proponente
                        if (!firmato_dal_proponente && em.UIDPersonaProponente.Value != persona.UID_persona)
                        {
                            results.Add(idGuid, $"ERROR: Il Proponente non ha ancora firmato l'emendamento {n_em}");
                            continue;
                        }

                        var info_codice_carica_gruppo = string.Empty;
                        switch (persona.CurrentRole)
                        {
                            case RuoliIntEnum.Consigliere_Regionale:
                                info_codice_carica_gruppo = persona.Gruppo.codice_gruppo;
                                break;
                            case RuoliIntEnum.Assessore_Sottosegretario_Giunta:
                                info_codice_carica_gruppo = persona.Carica;
                                break;
                        }

                        var isRelatore = _unitOfWork.Persone.IsRelatore(persona.UID_persona, em.ATTI.UIDAtto);
                        var isAssessore = _unitOfWork.Persone.IsAssessore(persona.UID_persona, em.ATTI.UIDAtto);

                        var bodyFirmaCert =
                            $"{persona.DisplayName} ({info_codice_carica_gruppo}){(isRelatore ? " - RELATORE" : string.Empty)}{(isAssessore ? " - Ass. capofila" : string.Empty)}";
                        firmaCert = EncryptString(bodyFirmaCert
                            , AppSettingsConfiguration.masterKey);
                    }

                    var dataFirma = EncryptString(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        AppSettingsConfiguration.masterKey);

                    var countFirme = _unitOfWork.Firme.CountFirme(idGuid);
                    if (countFirme == 0)
                    {
                        //Se è la prima firma dell'emendamento, questo viene cryptato e così certificato e non modificabile
                        em.Hash = firmaUfficio
                            ? EncryptString(AppSettingsConfiguration.MasterPIN, AppSettingsConfiguration.masterKey)
                            : pin.PIN;
                        em.UIDPersonaPrimaFirma = persona.UID_persona;
                        em.DataPrimaFirma = DateTime.Now;
                        var body = await GetBodyEM(em, new List<FIRME>
                            {
                                new FIRME
                                {
                                    UIDEM = idGuid,
                                    UID_persona = persona.UID_persona,
                                    FirmaCert = firmaCert,
                                    Data_firma = dataFirma,
                                    ufficio = firmaUfficio
                                }
                            }, persona,
                            TemplateTypeEnum.HTML);
                        var body_encrypt = EncryptString(body,
                            firmaUfficio ? AppSettingsConfiguration.MasterPIN : pin.PIN_Decrypt);

                        em.EM_Certificato = body_encrypt;
                    }

                    _unitOfWork.Firme.Firma(idGuid, persona.UID_persona, firmaCert, dataFirma, firmaUfficio);
                    await _unitOfWork.CompleteAsync();

                    results.Add(idGuid, "OK");
                }

                return results;
            }
            catch (Exception e)
            {
                Log.Error("Logic - FirmaEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region RitiroFirmaEmendamento

        public async Task<Dictionary<Guid, string>> RitiroFirmaEmendamento(ComandiAzioneModel firmaModel,
            PersonaDto persona)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                foreach (var idGuid in firmaModel.ListaEmendamenti)
                {
                    var em = GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    var n_em = GetNomeEM(Mapper.Map<EM, EmendamentiDto>(em),
                        em.Rif_UIDEM.HasValue
                            ? Mapper.Map<EM, EmendamentiDto>(GetEM(em.Rif_UIDEM.Value))
                            : null);

                    var seduta = _unitOfWork.Sedute.Get(em.ATTI.UIDSeduta.Value);

                    var ruoloSegreterie = _unitOfWork.Ruoli.Get((int) RuoliIntEnum.Segreteria_Assemblea);
                    var countFirme = _unitOfWork.Firme.CountFirme(idGuid);
                    if (countFirme == 1)
                    {
                        if (DateTime.Now > seduta.Data_seduta)
                        {
                            results.Add(idGuid,
                                "ERROR: Non è possibile ritirare l'ultima firma, in quanto equivale al ritiro dell'emendamento: annuncia in Aula l'intenzione di ritiro della firma");
                            continue;
                        }

                        //RITIRA EM
                        em.IDStato = (int) StatiEnum.Ritirato;
                        em.UIDPersonaRitiro = persona.UID_persona;
                        em.DataRitiro = DateTime.Now;

                        if (DateTime.Now > seduta.Scadenza_presentazione.Value && DateTime.Now < seduta.Data_seduta)
                            //SEND MAIL
                            _logicUtil.InvioMail(new MailModel
                            {
                                DA = "pem@consiglio.regione.lombardia.it",
                                A = $"{ruoloSegreterie.ADGroup}@consiglio.regione.lombardia.it",
                                OGGETTO =
                                    $"Ritirata ultima firma dall' {n_em} nel {em.ATTI.TIPI_ATTO.Tipo_Atto} {em.ATTI.NAtto}",
                                MESSAGGIO =
                                    "E' stata ritirata l'ultima firma all'emendamento in oggetto. Verifica lo stato dell'emendamento."
                            });
                    }

                    //RITIRA FIRMA
                    var firmeAttive = _unitOfWork
                        .Firme
                        .GetFirmatari(em, FirmeTipoEnum.ATTIVI);
                    FIRME firma_utente;
                    if (persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                        || persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea)
                        firma_utente = firmeAttive.Single(f => f.ufficio);
                    else
                        firma_utente = firmeAttive.Single(f => f.UID_persona == persona.UID_persona);

                    firma_utente.Data_ritirofirma =
                        EncryptString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            AppSettingsConfiguration.masterKey);

                    if (DateTime.Now > seduta.Scadenza_presentazione.Value)
                        _logicUtil.InvioMail(new MailModel
                        {
                            DA = "pem@consiglio.regione.lombardia.it",
                            A = $"{ruoloSegreterie.ADGroup}@consiglio.regione.lombardia.it",
                            OGGETTO =
                                $"Ritirata una firma dall' {n_em} nel {em.ATTI.TIPI_ATTO.Tipo_Atto} {em.ATTI.NAtto}",
                            MESSAGGIO = "E' stata ritirata una firma all'emendamento in oggetto."
                        });

                    await _unitOfWork.CompleteAsync();
                    results.Add(idGuid, "OK");
                }


                return results;
            }
            catch (Exception e)
            {
                Log.Error("Logic - RitiroFirmaEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region EliminaFirmaEmendamento

        public async Task<Dictionary<Guid, string>> EliminaFirmaEmendamento(ComandiAzioneModel firmaModel,
            PersonaDto persona)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                foreach (var idGuid in firmaModel.ListaEmendamenti)
                {
                    var em = GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    var countFirme = _unitOfWork.Firme.CountFirme(idGuid);
                    if (countFirme == 1)
                    {
                        em.EM_Certificato = string.Empty;
                        em.DataPrimaFirma = null;
                        em.UIDPersonaPrimaFirma = null;
                    }

                    //RITIRA FIRMA
                    var firmeAttive = _logicFirme.GetFirme(em, FirmeTipoEnum.ATTIVI);
                    var firma_utente = firmeAttive.Single(f => f.UID_persona == persona.UID_persona);

                    _unitOfWork.Firme.Remove(firma_utente);

                    results.Add(idGuid, "OK");
                }

                await _unitOfWork.CompleteAsync();
                return results;
            }
            catch (Exception e)
            {
                Log.Error("Logic - EliminaFirmaEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region DepositaEmendamento

        public async Task<Dictionary<Guid, string>> DepositaEmendamento(ComandiAzioneModel depositoModel,
            PersonaDto persona)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                BloccaDepositi = true;
                foreach (var idGuid in depositoModel.ListaEmendamenti)
                {
                    var em = GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    var emDto = await GetEM_DTO(em, persona);
                    var n_em = emDto.DisplayTitle;
                    if (emDto.STATI_EM.IDStato >= (int) StatiEnum.Depositato)
                    {
                        results.Add(idGuid, $"ERROR: Emendamento {n_em} già depositato");
                        continue;
                    }

                    var depositabile = _unitOfWork.Emendamenti.CheckIfDepositabile(emDto, persona);
                    if (!depositabile)
                    {
                        results.Add(idGuid, $"ERROR: Emendamento {n_em} non depositabile");
                        continue;
                    }

                    var etichetta_progressiva =
                        _unitOfWork.Emendamenti.GetEtichetta(emDto.UIDAtto, emDto.Rif_UIDEM.HasValue) + 1;
                    var etichetta_encrypt =
                        EncryptString(etichetta_progressiva.ToString(), AppSettingsConfiguration.masterKey);

                    var checkProgressivo_unique =
                        _unitOfWork.Emendamenti.CheckProgressivo(emDto.UIDAtto, etichetta_encrypt,
                            emDto.Rif_UIDEM.HasValue ? CounterEmendamentiEnum.SUB_EM : CounterEmendamentiEnum.EM);

                    if (!checkProgressivo_unique)
                    {
                        results.Add(idGuid, $"ERROR: Progressivo {n_em} occupato");
                        continue;
                    }

                    em.UIDPersonaDeposito = persona.UID_persona;
                    em.OrdinePresentazione = etichetta_progressiva;
                    em.Timestamp = DateTime.Now;
                    em.DataDeposito = EncryptString(em.Timestamp.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        AppSettingsConfiguration.masterKey);
                    em.IDStato = (int) StatiEnum.Depositato;
                    if (em.Rif_UIDEM.HasValue)
                        em.N_SUBEM = etichetta_encrypt;
                    else
                        em.N_EM = etichetta_encrypt;

                    em.chkf = _unitOfWork.Firme.CountFirme(idGuid).ToString();

                    await _unitOfWork.CompleteAsync();

                    results.Add(idGuid, "OK");

                    _unitOfWork.Stampe.Add(new STAMPE
                    {
                        UIDStampa = Guid.NewGuid(),
                        UIDUtenteRichiesta = persona.UID_persona,
                        CurrentRole = (int) persona.CurrentRole,
                        DataRichiesta = DateTime.Now,
                        UIDAtto = em.UIDAtto,
                        Da = 1,
                        A = 1,
                        Ordine = 1,
                        NotificaDepositoEM = true,
                        Scadenza = DateTime.Now.AddDays(Convert.ToDouble(AppSettingsConfiguration.GiorniValiditaLink)),
                        QueryEM =
                            $@"SELECT e.*, te.Tipo_EM,se.Stato,se.CssClass,ut.id_persona as IDProponente, ut.nome + ' ' + ut.cognome as Proponente, a.Articolo, c.Comma, pt.Parte, gp.codice_gruppo, gp.nome_gruppo,ap.AreaPolitica as DescrAreaPolitica
                    FROM EM e LEFT JOIN TIPI_EM te ON e.IDTipo_EM = te.IDTipo_EM LEFT JOIN STATI_EM se ON e.IDStato = se.IDStato LEFT JOIN View_UTENTI ut ON e.UIDPersonaProponente = ut.UID_persona
                    LEFT JOIN ARTICOLI a ON e.UIDArticolo = a.UIDArticolo LEFT JOIN COMMI c ON e.UIDComma = c.UIDComma LEFT JOIN PARTI_TESTO pt ON e.IDParte = pt.IDParte
                    LEFT JOIN gruppi_politici gp ON e.id_gruppo = gp.id_gruppo
                    LEFT JOIN AREAPOLITICA ap ON e.AreaPolitica = ap.Id
                    WHERE e.UIDEM = '{idGuid}' AND e.DataDeposito IS NOT NULL"
                    });
                    await _unitOfWork.CompleteAsync();
                }

                return results;
            }
            catch (Exception e)
            {
                Log.Error("Logic - DepositaEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region RitiraEmendamento

        public async Task RitiraEmendamento(EM em, PersonaDto persona)
        {
            try
            {
                em.IDStato = (int) StatiEnum.Ritirato;
                em.UIDPersonaRitiro = persona.UID_persona;
                em.DataRitiro = DateTime.Now;

                if (DateTime.Now > em.ATTI.SEDUTE.Scadenza_presentazione &&
                    DateTime.Now < em.ATTI.SEDUTE.Data_seduta)
                {
                    // INVIO MAIL A SEGRETERIA PER AVVISARE DEL RITIRO DELL'EM DOPO IL TERMINE DELL'ATTO
                    var nome_em = GetNomeEM(
                        Mapper.Map<EM, EmendamentiDto>(em),
                        em.Rif_UIDEM.HasValue
                            ? Mapper.Map<EM, EmendamentiDto>(GetEM(em.Rif_UIDEM.Value))
                            : null);
                    var ruoloSegreterie = _unitOfWork.Ruoli.Get(10);
                    _logicUtil.InvioMail(new MailModel
                    {
                        DA = "pem@consiglio.regione.lombardia.it",
                        A = $"{ruoloSegreterie.ADGroup}@consiglio.regione.lombardia.it",
                        OGGETTO = $"Ritirato {nome_em} nel {em.ATTI.TIPI_ATTO.Tipo_Atto} {em.ATTI.NAtto}",
                        MESSAGGIO = "ATTENZIONE: E' stato appena ritirato l'emendamento in oggetto"
                    });
                }
                else if (DateTime.Now > em.ATTI.SEDUTE.Data_seduta)
                {
                    throw new Exception(
                        "Non è possibile ritirare l'emendamento durante lo svolgimento della seduta: annuncia in Aula l'intenzione di ritiro");
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception e)
            {
                Log.Error("Logic - RitiraEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region ModificaStatoEmendamento

        public async Task<Dictionary<Guid, string>> ModificaStatoEmendamento(ModificaStatoModel model)
        {
            try
            {
                var results = new Dictionary<Guid, string>();

                foreach (var idGuid in model.ListaEmendamenti)
                {
                    var em = GetEM(idGuid);
                    if (em == null)
                    {
                        results.Add(idGuid, "ERROR: NON TROVATO");
                        continue;
                    }

                    em.IDStato = (int) model.Stato;
                    await _unitOfWork.CompleteAsync();
                    results.Add(idGuid, "OK");
                    try
                    {
                        //OPENDATA
                        if (AppSettingsConfiguration.AbilitaOpenData == "1")
                        {
                            var wsOD = new UpsertOpenData();
                            var firme = _logicFirme.GetFirme(em, FirmeTipoEnum.TUTTE);
                            var firmeDto = firme.Select(Mapper.Map<FIRME, FirmeDto>).ToList();

                            var resultOpenData = GetEM_OPENDATA(em,
                                em.Rif_UIDEM.HasValue ? GetEM(em.Rif_UIDEM.Value) : null,
                                firmeDto,
                                Mapper.Map<View_UTENTI, PersonaDto>(
                                    _unitOfWork.Persone.Get(em.UIDPersonaProponente.Value)));
                            wsOD.UpsertEM(resultOpenData, AppSettingsConfiguration.OpenData_PrivateToken);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("OpenDataEM", e);
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                Log.Error("Logic - ModificaStatoEmendamento", e);
                throw e;
            }
        }

        #endregion

        #region GetEM

        public EM GetEM(Guid id)
        {
            return _unitOfWork.Emendamenti.Get(id);
        }

        public EM GetEM(string id)
        {
            var guidId = new Guid(id);
            return GetEM(guidId);
        }

        #endregion

        #region GetEM_DTO

        public async Task<EmendamentiDto> GetEM_DTO(Guid emendamentoUId, PersonaDto persona)
        {
            var em = GetEM(emendamentoUId);
            return await GetEM_DTO(em, persona);
        }

        public async Task<EmendamentiDto> GetEM_DTO(EM em, PersonaDto persona)
        {
            try
            {
                var emendamentoDto = Mapper.Map<EM, EmendamentiDto>(em);

                if (persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea
                    || persona.CurrentRole == RuoliIntEnum.Amministratore_PEM
                    && string.IsNullOrEmpty(emendamentoDto.TestoEM_Modificabile))
                    emendamentoDto.TestoEM_Modificabile = emendamentoDto.TestoEM_originale;

                emendamentoDto.AbilitaSUBEM = emendamentoDto.STATI_EM.IDStato == (int) StatiEnum.Depositato
                                              && emendamentoDto.UIDPersonaProponente.Value != persona.UID_persona
                                              && !emendamentoDto.ATTI.Chiuso ||
                                              persona.CurrentRole == RuoliIntEnum.Amministratore_PEM &&
                                              persona.CurrentRole == RuoliIntEnum.Segreteria_Assemblea;
                emendamentoDto.DisplayTitle = GetNomeEM(emendamentoDto,
                    emendamentoDto.Rif_UIDEM.HasValue
                        ? Mapper.Map<EM, EmendamentiDto>(GetEM(emendamentoDto.Rif_UIDEM.Value))
                        : null);
                emendamentoDto.ConteggioFirme = _logicFirme.CountFirme(emendamentoDto.UIDEM);
                if (!string.IsNullOrEmpty(emendamentoDto.DataDeposito))
                    emendamentoDto.DataDeposito = Decrypt(emendamentoDto.DataDeposito);
                if (!string.IsNullOrEmpty(emendamentoDto.EM_Certificato))
                    emendamentoDto.EM_Certificato = Decrypt(emendamentoDto.EM_Certificato, em.Hash);
                emendamentoDto.Firma_da_ufficio = _unitOfWork.Firme.CheckFirmatoDaUfficio(emendamentoDto.UIDEM);
                emendamentoDto.Firmato_Dal_Proponente = em.STATI_EM.IDStato >= (int) StatiEnum.Depositato
                    ? true
                    : _unitOfWork.Firme.CheckFirmato(em.UIDEM, em.UIDPersonaProponente.Value);
                if (emendamentoDto.ConteggioFirme > 1)
                {
                    var firme = _logicFirme.GetFirme(emendamentoDto, FirmeTipoEnum.ATTIVI);
                    emendamentoDto.Firme = firme
                        .Where(f => f.UID_persona != emendamentoDto.UIDPersonaProponente)
                        .Select(f => f.FirmaCert)
                        .Aggregate((i, j) => i + "<br>" + j);
                }

                if (string.IsNullOrEmpty(em.DataDeposito))
                    emendamentoDto.Depositabile = _unitOfWork
                        .Emendamenti
                        .CheckIfDepositabile(emendamentoDto,
                            persona);

                if (em.STATI_EM.IDStato <= (int) StatiEnum.Depositato)
                    emendamentoDto.Firmabile = _unitOfWork
                        .Firme
                        .CheckIfFirmabile(emendamentoDto,
                            persona);
                if (!em.DataRitiro.HasValue && em.STATI_EM.IDStato == (int) StatiEnum.Depositato)
                    emendamentoDto.Ritirabile = _unitOfWork
                        .Emendamenti
                        .CheckIfRitirabile(emendamentoDto,
                            persona);
                if (string.IsNullOrEmpty(em.DataDeposito))
                    emendamentoDto.Eliminabile = _unitOfWork
                        .Emendamenti
                        .CheckIfEliminabile(emendamentoDto,
                            persona);

                emendamentoDto.Modificabile = _unitOfWork
                    .Emendamenti
                    .CheckIfModificabile(emendamentoDto,
                        persona);

                emendamentoDto.Invito_Abilitato = _unitOfWork
                    .Notifiche
                    .CheckIfNotificabile(emendamentoDto,
                        persona);

                emendamentoDto.PersonaProponente =
                    Mapper.Map<View_UTENTI, PersonaLightDto>(
                        _unitOfWork.Persone.Get(emendamentoDto.UIDPersonaProponente.Value));
                emendamentoDto.PersonaCreazione =
                    Mapper.Map<View_UTENTI, PersonaLightDto>(
                        _unitOfWork.Persone.Get(emendamentoDto.UIDPersonaCreazione.Value));
                if (!string.IsNullOrEmpty(emendamentoDto.DataDeposito))
                    emendamentoDto.PersonaDeposito =
                        Mapper.Map<View_UTENTI, PersonaLightDto>(
                            _unitOfWork.Persone.Get(emendamentoDto.UIDPersonaDeposito.Value));

                if (emendamentoDto.UIDPersonaModifica.HasValue)
                    emendamentoDto.PersonaModifica =
                        Mapper.Map<View_UTENTI, PersonaLightDto>(
                            _unitOfWork.Persone.Get(emendamentoDto.UIDPersonaModifica.Value));

                return emendamentoDto;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetEM_DTO", e);
                throw e;
            }
        }

        #endregion

        #region GetEmendamenti

        public async Task<List<EmendamentiDto>> GetEmendamenti(BaseRequest<EmendamentiDto> model,
            PersonaDto persona, int CLIENT_MODE)
        {
            try
            {
                var queryFilter = new Filter<EM>();
                queryFilter.ImportStatements(model.filtro);

                var emendamentiDtos = _unitOfWork
                    .Emendamenti
                    .GetAll(model.id,
                        persona,
                        model.ordine,
                        model.page,
                        model.size,
                        CLIENT_MODE,
                        queryFilter)
                    .ToList();
                var result = new List<EmendamentiDto>();
                foreach (var emendamentoUId in emendamentiDtos) result.Add(await GetEM_DTO(emendamentoUId, persona));

                return result;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetEmendamenti", e);
                throw e;
            }
        }

        public async Task<List<EmendamentiDto>> GetEmendamenti(EmendamentiByQueryModel model)
        {
            try
            {
                var emendamentiDtos = _unitOfWork
                    .Emendamenti
                    .GetAll(model)
                    .Select(Mapper.Map<EM, EmendamentiDto>)
                    .ToList();

                emendamentiDtos.ForEach(delegate(EmendamentiDto em)
                {
                    em.DisplayTitle = GetNomeEM(em,
                        em.Rif_UIDEM.HasValue
                            ? Mapper.Map<EM, EmendamentiDto>(GetEM(em.Rif_UIDEM.Value))
                            : null);
                    em.DataDeposito = !string.IsNullOrEmpty(em.DataDeposito) ? Decrypt(em.DataDeposito) : "";
                });

                return emendamentiDtos;
            }
            catch (Exception e)
            {
                Log.Error("Logic - GetEmendamenti", e);
                throw e;
            }
        }

        #endregion

        #region CountEM

        public int CountEM(BaseRequest<EmendamentiDto> model, PersonaDto persona, int CLIENT_MODE,
            CounterEmendamentiEnum type = CounterEmendamentiEnum.NONE)
        {
            try
            {
                var queryFilter = new Filter<EM>();
                queryFilter.ImportStatements(model.filtro);

                return _unitOfWork.Emendamenti.Count(model.id,
                    persona, type, CLIENT_MODE, queryFilter);
            }
            catch (Exception e)
            {
                Log.Error("Logic - CountEM", e);
                throw e;
            }
        }

        public int CountEM(string query)
        {
            try
            {
                return _unitOfWork.Emendamenti.Count(query);
            }
            catch (Exception e)
            {
                Log.Error("Logic - CountEM", e);
                throw e;
            }
        }

        #endregion
    }
}