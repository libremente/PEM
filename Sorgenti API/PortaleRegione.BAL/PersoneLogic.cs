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

        public PinDto GetPin(PersonaDto persona)
        {
            var pin = Mapper.Map<View_PINS, PinDto>(_unitOfWork.Persone.GetPin(persona.UID_persona));
            pin.PIN_Decrypt = Decrypt(pin.PIN);
            return pin;
        }

        #endregion

        #region CambioPin

        public async Task CambioPin(CambioPinModel model, PersonaDto persona)
        {
            _unitOfWork.Persone.SavePin(persona.UID_persona,
                EncryptString(model.nuovo_pin, AppSettingsConfiguration.masterKey),
                Convert.ToBoolean(persona.No_Cons),
                model.reset);
            await _unitOfWork.CompleteAsync();
        }

        #endregion

        #region GetGruppoAttualePersona

        public GruppiDto GetGruppoAttualePersona(PersonaDto persona, bool isGiunta)
        {
            return Mapper.Map<View_gruppi_politici_con_giunta, GruppiDto>(
                _unitOfWork.Gruppi.GetGruppoAttuale(persona, isGiunta));
        }

        #endregion

        #region GetConsiglieri

        public IEnumerable<PersonaDto> GetConsiglieri()
        {
            var result = _unitOfWork
                .Persone
                .GetConsiglieri(_unitOfWork.Legislature.Legislatura_Attiva())
                .Select(Mapper.Map<View_UTENTI, PersonaDto>).ToList();

            result.ForEach(persona => { persona.Gruppo ??= GetGruppoAttualePersona(persona, false); });
            return result;
        }

        #endregion

        #region GetAssessoriRiferimento

        public IEnumerable<PersonaDto> GetAssessoriRiferimento()
        {
            var result = _unitOfWork
                .Persone
                .GetAssessoriRiferimento(_unitOfWork.Legislature.Legislatura_Attiva())
                .Select(Mapper.Map<View_UTENTI, PersonaDto>);
            return result;
        }

        #endregion

        #region GetConsiglieriGruppo

        public IEnumerable<PersonaDto> GetConsiglieriGruppo(int gruppoId)
        {
            var result = _unitOfWork
                .Gruppi
                .GetConsiglieriGruppo(_unitOfWork.Legislature.Legislatura_Attiva(),
                    gruppoId)
                .Select(Mapper.Map<View_UTENTI, PersonaDto>);
            return result;
        }

        #endregion

        #region GetPersona

        public PersonaDto GetPersona(Guid proponenteUId, bool isGiunta)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(_unitOfWork.Persone.Get(proponenteUId));
            persona.Gruppo = GetGruppoAttualePersona(persona, isGiunta);
            return persona;
        }

        public PersonaDto GetPersona(int personaId)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(_unitOfWork.Persone.Get(personaId));
            //persona.Gruppo = GetGruppoAttualePersona(persona, isGiunta);
            return persona;
        }

        #endregion

        #region GetCapoGruppo

        public PersonaDto GetCapoGruppo(int gruppoId)
        {
            var persona = Mapper.Map<View_UTENTI, PersonaDto>(_unitOfWork.Gruppi.GetCapoGruppo(gruppoId));
            return persona;
        }

        #endregion

        #region GetPersone_DA_CANCELLARE

        protected internal IEnumerable<PersonaDto> GetPersone_DA_CANCELLARE()
        {
            return _unitOfWork.Persone.GetAll_DA_CANCELLARE().Select(Mapper.Map<View_UTENTI, PersonaDto>);
        }

        #endregion

        #region GetGruppi

        public IEnumerable<KeyValueDto> GetGruppi()
        {
            var gruppiDtos = _unitOfWork
                .Gruppi
                .GetAll(_unitOfWork
                    .Legislature
                    .Legislatura_Attiva());

            return gruppiDtos;
        }

        #endregion

        #region GetRelatori

        public IEnumerable<PersonaDto> GetRelatori(Guid? id)
        {
            var personaDtos = _unitOfWork.Persone
                .GetRelatori(id == null || id == Guid.Empty ? Guid.Empty : id)
                .Select(Mapper.Map<View_UTENTI, PersonaDto>);

            return personaDtos;
        }

        #endregion

        #region GetCaricaPersona

        public string GetCaricaPersona(Guid personaUId)
        {
            return _unitOfWork.Persone.GetCarica(personaUId);
        }

        #endregion
    }
}