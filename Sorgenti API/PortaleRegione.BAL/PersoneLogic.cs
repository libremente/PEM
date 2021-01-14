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
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PortaleRegione.Contracts;
using PortaleRegione.Domain;
using PortaleRegione.DTO.Domain;
using PortaleRegione.DTO.Model;

namespace PortaleRegione.BAL
{
    public class PersoneLogic : BaseLogic
    {
        #region ctor

        public PersoneLogic(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region GetPin

        public async Task<PinDto> GetPin(PersonaDto persona)
        {
            var pinInDb = await _unitOfWork.Persone.GetPin(persona.UID_persona);
            if (pinInDb == null)
                return null;
            var pin = Mapper.Map<View_PINS, PinDto>(pinInDb);
            pin.PIN_Decrypt = Decrypt(pin.PIN);
            return pin;
        }

        #endregion

        #region CambioPin

        public async Task CambioPin(CambioPinModel model)
        {
            await _unitOfWork.Persone.SavePin(model.PersonaUId,
                EncryptString(model.nuovo_pin, AppSettingsConfiguration.masterKey),
                false);
            await _unitOfWork.CompleteAsync();
        }

        #endregion

        #region ResetPin

        public async Task ResetPin(ResetPinModel model)
        {
            await _unitOfWork.Persone.SavePin(model.PersonaUId,
                EncryptString(model.nuovo_pin, AppSettingsConfiguration.masterKey),
                true);
            await _unitOfWork.CompleteAsync();
        }

        #endregion

        #region GetGruppoAttualePersona

        public async Task<GruppiDto> GetGruppoAttualePersona(PersonaDto persona, bool isGiunta)
        {
            return Mapper.Map<View_gruppi_politici_con_giunta, GruppiDto>(
                await _unitOfWork.Gruppi.GetGruppoAttuale(persona, isGiunta));
        }

        #endregion

        #region GetConsiglieri

        public async Task<IEnumerable<PersonaDto>> GetConsiglieri()
        {
            var result = (await _unitOfWork
                    .Persone
                    .GetConsiglieri(await _unitOfWork.Legislature.Legislatura_Attiva()))
                .Select(Mapper.Map<View_UTENTI, PersonaDto>).ToList();

            result.ForEach(async persona => { persona.Gruppo ??= await GetGruppoAttualePersona(persona, false); });
            return result;
        }

        #endregion

        #region GetAssessoriRiferimento

        public async Task<IEnumerable<PersonaDto>> GetAssessoriRiferimento()
        {
            var result = (await _unitOfWork
                .Persone
                .GetAssessoriRiferimento(await _unitOfWork.Legislature.Legislatura_Attiva()))
                .Select(Mapper.Map<View_UTENTI, PersonaDto>);
            return result;
        }

        #endregion

        #region GetConsiglieriGruppo

        public async Task<IEnumerable<PersonaDto>> GetConsiglieriGruppo(int gruppoId)
        {
            var result = (await _unitOfWork
                .Gruppi
                .GetConsiglieriGruppo(await _unitOfWork.Legislature.Legislatura_Attiva(),
                    gruppoId))
                .Select(Mapper.Map<View_UTENTI, PersonaDto>);
            return result;
        }

        #endregion

        #region GetCapoGruppo

        public async Task<PersonaDto> GetCapoGruppo(int gruppoId)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(await _unitOfWork.Gruppi.GetCapoGruppo(gruppoId));
            return persona;
        }

        #endregion

        #region GetPersone_DA_CANCELLARE

        protected internal async Task<IEnumerable<PersonaDto>> GetPersone_DA_CANCELLARE()
        {
            return (await _unitOfWork.Persone.GetAll_DA_CANCELLARE()).Select(Mapper.Map<View_UTENTI, PersonaDto>);
        }

        #endregion

        #region GetGruppi

        public async Task<IEnumerable<KeyValueDto>> GetGruppi()
        {
            var gruppiDtos = await _unitOfWork
                .Gruppi
                .GetAll(await _unitOfWork
                    .Legislature
                    .Legislatura_Attiva());

            return gruppiDtos;
        }

        #endregion

        #region GetRelatori

        public async Task<IEnumerable<PersonaDto>> GetRelatori(Guid? id)
        {
            var personaDtos = (await _unitOfWork.Persone
                .GetRelatori(id == null || id == Guid.Empty ? Guid.Empty : id))
                .Select(Mapper.Map<View_UTENTI, PersonaDto>);

            return personaDtos;
        }

        #endregion

        #region GetCaricaPersona

        public async Task<string> GetCaricaPersona(Guid personaUId)
        {
            return await _unitOfWork.Persone.GetCarica(personaUId);
        }

        #endregion

        #region GetPersona

        public async Task<PersonaDto> GetPersona(Guid proponenteUId, bool isGiunta)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(await _unitOfWork.Persone.Get(proponenteUId));
            persona.Gruppo = await GetGruppoAttualePersona(persona, isGiunta);
            return persona;
        }

        public async Task<PersonaDto> GetPersona(int personaId)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(await _unitOfWork.Persone.Get(personaId));
            return persona;
        }

        public async Task<PersonaDto> GetPersona(Guid personaUId)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(await _unitOfWork.Persone.Get(personaUId));
            return persona;
        }

        #endregion
    }
}