
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

function filter_em_text1_OnChange() {
    var value = $("#filter_em_text1").val();
    var filtri_em = get_Filtri_EM();
    filtri_em.text1 = value;
    set_Filtri_EM(filtri_em);
}
