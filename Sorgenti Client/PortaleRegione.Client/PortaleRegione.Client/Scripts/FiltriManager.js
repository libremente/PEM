
/////////////////////////////////////////////////////////////
////////
////////            SEDUTE
////////
/////////////////////////////////////////////////////////////

async function Filtri_Sedute_CaricaLegislature(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_Sedute();
    if (filtri != null) {
        filterSelect = filtri.legislatura;
    }

    var legislature = await GetLegislature();
    if (legislature.length > 0) {
        var select = $("#" + ctrlSelect);
        select.empty();
        select.append('<option value="0">Seleziona</option>');

        $.each(legislature,
            function(index, item) {
                var template = "";
                if (item.id_legislatura == filterSelect)
                    template = "<option selected='selected'></option>";
                else
                    template = "<option></option>";
                select.append($(template).val(item.id_legislatura).html(item.num_legislatura));
            });

        var elems = document.querySelectorAll("#" + ctrlSelect);
        M.FormSelect.init(elems, null);
    }
}

async function Filtri_Sedute_CaricaAnni(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_Sedute();
    if (filtri != null) {
        filterSelect = filtri.anno;
    }
    var currentYear = new Date().getFullYear();
    var select = $("#" + ctrlSelect);
    select.empty();
    select.append('<option value="0">Seleziona</option>');

    for (var i = 5; i >= 0; i--) {
        var template = "";
        if (currentYear == filterSelect)
            template = "<option selected='selected'></option>";
        else
            template = "<option></option>";
        select.append($(template).val(currentYear).html(currentYear));
        currentYear--;
    }

    var elems = document.querySelectorAll("#" + ctrlSelect);
    M.FormSelect.init(elems, null);
}

async function Filtri_Sedute_CaricaDa(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_Sedute();
    if (filtri != null) {
        filterSelect = filtri.da;
    }

    var select = $("#" + ctrlSelect);
    select.empty();
    select.val(filterSelect);
}

async function Filtri_Sedute_CaricaA(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_Sedute();
    if (filtri != null) {
        filterSelect = filtri.a;
    }

    var select = $("#" + ctrlSelect);
    select.empty();
    select.val(filterSelect);
}

function GetLegislature() {
    var legislature = get_ListaLegislature();
    if (legislature.length > 0) {
        return legislature;
    }

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/sedute/legislature",
            type: "GET"
        }).done(function(result) {
            set_ListaLegislature(result);
            resolve(result);
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function filter_sedute_legislature_OnChange() {
    var value = $("#filter_sedute_legislature").children("option:selected").val();
    var filtri_sedute = get_Filtri_Sedute();
    filtri_sedute.legislatura = value;
    set_Filtri_Sedute(filtri_sedute);
}

function filter_sedute_anno_OnChange() {
    var value = $("#filter_sedute_anno").children("option:selected").val();
    var filtri_sedute = get_Filtri_Sedute();
    filtri_sedute.anno = value;
    set_Filtri_Sedute(filtri_sedute);
}

function filter_sedute_a_OnChange() {
    var value = $("#filter_sedute_a").val();
    var filtri_sedute = get_Filtri_Sedute();
    filtri_sedute.a = value;
    set_Filtri_Sedute(filtri_sedute);
}

function filter_sedute_da_OnChange() {
    var value = $("#filter_sedute_da").val();
    var filtri_sedute = get_Filtri_Sedute();
    filtri_sedute.da = value;
    set_Filtri_Sedute(filtri_sedute);
}

/////////////////////////////////////////////////////////////
////////
////////            EMENDAMENTI
////////
/////////////////////////////////////////////////////////////

async function Filtri_EM_CaricaText1(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_EM();
    if (filtri != null) {
        filterSelect = filtri.text1;
    }

    var select = $("#" + ctrlSelect);
    select.empty();
    select.val(filterSelect);
}

async function Filtri_EM_CaricaNumeroEM(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_EM();
    if (filtri != null) {
        filterSelect = filtri.n_em;
    }

    var select = $("#" + ctrlSelect);
    select.empty();
    select.val(filterSelect);
}

async function Filtri_EM_CaricaStatiEM(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_EM();
    if (filtri != null) {
        filterSelect = filtri.stato;
    }

    var stati = await GetStatiEM();
    if (stati.length > 0) {
        var select = $("#" + ctrlSelect);
        select.empty();
        select.append('<option value="-10">Seleziona</option>');
        $.each(stati,
            function(index, item) {
                var template = "";
                if (item.IDStato == filterSelect)
                    template = "<option selected='selected'></option>";
                else
                    template = "<option></option>";
                select.append($(template).val(item.IDStato).html(item.Stato));
            });

        var elems = document.querySelectorAll("#" + ctrlSelect);
        M.FormSelect.init(elems, null);
    }
}

function GetStatiEM() {
    var stati = get_ListaStatiEM();
    if (stati.length > 0) {
        return stati;
    }

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/emendamenti/stati-em",
            type: "GET"
        }).done(function(result) {
            set_ListaStatiEM(result);
            resolve(result);
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

async function Filtri_EM_CaricaTipiEM(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_EM();
    if (filtri != null) {
        filterSelect = filtri.tipo;
    }

    var tipi = await GetTipiEM();
    if (tipi.length > 0) {
        var select = $("#" + ctrlSelect);
        select.empty();
        select.append('<option value="-10">Seleziona</option>');

        $.each(tipi,
            function(index, item) {
                var template = "";
                if (item.IDTipo_EM == filterSelect)
                    template = "<option selected='selected'></option>";
                else
                    template = "<option></option>";
                select.append($(template).val(item.IDTipo_EM).html(item.Tipo_EM));
            });

        var elems = document.querySelectorAll("#" + ctrlSelect);
        M.FormSelect.init(elems, null);
    }
}

function GetTipiEM() {
    var tipi = get_ListaTipiEM();
    if (tipi.length > 0) {
        return tipi;
    }

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/emendamenti/tipi-em",
            type: "GET"
        }).done(function(result) {
            set_ListaTipiEM(result);
            resolve(result);
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

async function Filtri_EM_CaricaPartiEM(ctrlSelect) {
    var filterSelect = 0;
    var filtri = get_Filtri_EM();
    if (filtri != null) {
        filterSelect = filtri.parte;
    }

    var parti = await GetPartiEM();
    if (parti.length > 0) {
        var select = $("#" + ctrlSelect);
        select.empty();
        select.append('<option value="-10">Seleziona</option>');

        $.each(parti,
            function(index, item) {
                var template = "";
                if (item.IDParte == filterSelect)
                    template = "<option selected='selected'></option>";
                else
                    template = "<option></option>";
                select.append($(template).val(item.IDParte).html(item.Parte));
            });

        var elems = document.querySelectorAll("#" + ctrlSelect);
        M.FormSelect.init(elems, null);
    }
}

function GetPartiEM() {
    var parti = get_ListaPartiEM();
    if (parti.length > 0) {
        return parti;
    }

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/emendamenti/parti-em",
            type: "GET"
        }).done(function(result) {
            set_ListaPartiEM(result);
            resolve(result);
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function filter_em_text1_OnChange() {
    var value = $("#filter_em_text1").val();
    var filtri_em = get_Filtri_EM();
    filtri_em.text1 = value;
    set_Filtri_EM(filtri_em);
}

function filter_em_n_em_OnChange() {
    var value = $("#filter_em_n_em").val();
    var filtri_em = get_Filtri_EM();
    filtri_em.n_em = value;
    set_Filtri_EM(filtri_em);
}

function filter_em_stato_OnChange() {
    var value = $("#filter_em_stato").val();
    var filtri_em = get_Filtri_EM();
    filtri_em.stato = value;
    set_Filtri_EM(filtri_em);
}

function filter_em_tipo_OnChange() {
    var value = $("#filter_em_tipo").val();
    var filtri_em = get_Filtri_EM();
    filtri_em.tipo = value;
    set_Filtri_EM(filtri_em);
}

function filter_em_parte_OnChange() {
    var value = $("#filter_em_parte").val();
    var filtri_em = get_Filtri_EM();
    filtri_em.parte = value;
    set_Filtri_EM(filtri_em);
}