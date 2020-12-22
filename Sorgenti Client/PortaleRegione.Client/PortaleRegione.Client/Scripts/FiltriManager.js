function Reset_Filtri() {
    set_Filtri_Sedute({});
}

async function Filtri_CaricaLegislature(ctrlSelect) {
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