var baseUrl = "";

document.addEventListener("DOMContentLoaded",
    function() {
        // INITIALIZE MATERIALIZE v1.0.0 - https://materializecss.com/
        M.AutoInit();
        // TINYMCE v4
        // RICHIESTA CHIAVE API GRATUITA - https://www.tiny.cloud/
        tinymce.init({
            selector: ".tinymce"
        });

        var mode = getClientMode();
        if (mode == null)
            setClientMode(1);
    });

var gruppi_theme = "";
var trattazione_theme = "";

function AggiornaPosizioneTemi(baseUrl) {
    gruppi_theme = baseUrl + "/Content/gruppi-theme.css";
    trattazione_theme = baseUrl + "/Content/trattazione-theme.css";
}

function Reset_ClientMode() {
    setClientMode(1);
}

// Gestione apertura navigazione laterale DX (Ricerche/Filtri)
function openSearch() {
    var elems = document.querySelector("#slide-out-DX");
    var instances = M.Sidenav.init(elems,
        {
            edge: "right",
            draggable: true,
            onOpenStart: function() {
                // Menu laterale SX sotto a layer opaco
                $("#slide-out").css("z-index", 997);
            },
            onCloseEnd: function() {
                // Ripristino menu laterale SX
                $("#slide-out").css("z-index", 999);
            }
        });

    instances.open();
}

function go(link, switchMode) {
    var mode = getClientMode();
    if (switchMode == true) {
        if (link.includes("mode=1")) {
            link = link.replace("mode=1", "mode=2");
        } else if (link.includes("mode=2")) {
            link = link.replace("mode=2", "mode=1");
        } else if (link.includes("?")) {
            link = link + "&mode=" + mode;
        } else {
            link = link + "?mode=" + mode;
        }
    } else {
        if (link.includes("mode")) {
            //esco
        } else if (link.includes("?")) {
            link = link + "&mode=" + mode;
        } else {
            link = link + "?mode=" + mode;
        }
    }

    document.location = link;
}

function AbilitaTrattazione(url) {
    var mode = getClientMode();
    if (mode == 1) {
        setClientMode(2);
    } else {
        setClientMode(1);
    }
    go(url, true);
}

//EVENTI EMENDAMENTO

function DeselectALLEM() {
    $("#checkAll").prop("checked", false);
    $('input[id^="chk_EM_"]').not(this).prop("checked", false);
    setSelezionaTutti(false);
    setListaEmendamenti([]);
    AbilitaComandiMassivi(null);
}

function AbilitaComandiMassivi(uidEM) {
    if (uidEM) {
        var chk = $("#chk_EM_" + uidEM);
        if (chk.length > 0) {
            var selezionaTutti = getSelezionaTutti();
            if (chk[0].checked) {
                if (selezionaTutti) {
                    removeEM(uidEM); //listaEsclusiva
                } else {
                    addEM(uidEM); //listaInsclusiva
                }
            } else {
                if (selezionaTutti) {
                    addEM(uidEM); //listaEsclusiva
                } else {
                    removeEM(uidEM); //listaInsclusiva
                }
            }
        }
    }

    var lchk = getListaEmendamenti();

    if (lchk.length > 0 || $("#checkAll")[0].checked) {
        $("#btnComandiMassiviAdmin").show();
        $("#btnComandiMassiviOrdinamentoAdmin").show();
        $("#btnComandiMassivi").show();
        $("#btnNuovoEM").hide();
    } else {
        $("#btnComandiMassiviAdmin").hide();
        $("#btnComandiMassiviOrdinamentoAdmin").hide();
        $("#btnComandiMassivi").hide();
        $("#btnNuovoEM").show();
    }
}

function checkSelectedEM() {
    var selezionaTutti = getSelezionaTutti();
    var lista = getListaEmendamenti();

    $("#checkAll").prop("checked", selezionaTutti);
    $('input[id^="chk_EM_"]').not(this).prop("checked", selezionaTutti);
    $.each(lista,
        function(index, item) {
            $("#chk_EM_" + item).prop("checked", selezionaTutti ? false : true);
        });

    AbilitaComandiMassivi(null);
}

function addEM(uidEM) {
    var lista = getListaEmendamenti();
    lista.push(uidEM);
    setListaEmendamenti(lista);
}

function removeEM(uidEM) {
    var lista = getListaEmendamenti();
    var findIndex = jQuery.inArray(uidEM, lista);
    lista.splice(findIndex, 1);
    setListaEmendamenti(lista);
}

function ConfirmAction(id, name, action) {
    $("#emActionDisplayName").empty();
    $("#emActionDisplayName").append(name);
    $("#emActionMessage").empty();

    if (action == 1) {
        $("#btnConfermaAction").text("ELIMINA");
        $("#emActionMessage").append("Stai per eliminare l'emendamento selezionato. Sei sicuro?");
    } else if (action == 2) {
        $("#btnConfermaAction").text("RITIRA");
        $("#emActionMessage").append("Stai per ritirare l'emendamento selezionato. Sei sicuro?");
    }
    $("#btnConfermaAction").on("click",
        function() {
            $.ajax({
                url: baseUrl + "/emendamenti/azioni?id=" + id + "&azione=" + action,
                method: "GET"
            }).done(function(data) {
                $("#modalAction").modal("close");
                $("#btnConfermaAction").off("click");

                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    go(data);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
    $("#modalAction").modal("open");
}

function RitiraFirma(id) {

    swal("Inserisci il pin per ritirare la firma",
            {
                content: {
                    element: "input",
                    attributes: { placeholder: "******", className: "password" }
                },
                buttons: { cancel: "Annulla", confirm: "Si" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/emendamenti/ritiro-firma?id=" + id + "&pin=" + value,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    go(data);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function EliminaFirma(id) {

    swal("Inserisci il pin per eliminare la firma",
            {
                content: {
                    element: "input",
                    attributes: { placeholder: "******", className: "password" }
                },
                buttons: { cancel: "Annulla", confirm: "Si" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/emendamenti/elimina-firma?id=" + id + "&pin=" + value,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    go(data);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function RevealFirmatari(uidem) {
    var panel = $("#reveal_" + uidem + " #dataItems");
    panel.show();
    var panelArea = $("#reveal_" + uidem + " #PinArea");
    panelArea.hide();
    $("#titleReveal").text("Firmatari");
    return new Promise(function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/emendamenti/firmatari",
            data: { id: uidem, tipo: 3, tag: true },
            type: "GET"
        }).done(function(data) {
            panel.empty();
            if (data.length > 0) {
                panel.append(data);
            } else {
                //Pannello di cortesia
                panel.append(
                    "<div>L'emendamento non è stato ancora firmato</div>");
            }
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function RevealFirmaDeposito(id, action) {
    var text = "";
    var button = "";
    if (action == 3) {
        text = "Inserisci il PIN per firmare";
        button = "Firma";
    } else if (action == 4) {
        text = "Inserisci il PIN per depositare";
        button = "Deposita";
    }

    swal(text,
            {
                content: {
                    element: "input",
                    attributes: { placeholder: "******", className: "password" }
                },
                buttons: { cancel: "Annulla", confirm: button }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/emendamenti/azioni?id=" + id + "&azione=" + action + "&pin=" + value,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    go(data);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}


function TipoEmendamento_OnChange(item) {
    TestoEmendamento_TipoEM(item.value);
}

function PartiTestoEmendamento_OnChange(item) {

    ClearMetaDati();

    var viewPanel = true;
    if (item == "2")
        $("#pnlTitolo").show();
    else if (item == "3")
        $("#pnlCapo").show();
    else if (item == "4")
        $("#pnlArticoli").show();
    else if (item == "5")
        $("#pnlMissioni").show();
    else
        viewPanel = false;

    if (viewPanel == true)
        $("#detailsPanel").show();
    else
        $("#detailsPanel").hide();
}

function EffettiFinanziari_OnChange(change) {
    console.log("EffettiFinanziari_OnChange", change);
}

function NTitolo_OnChange(item) {
    TestoEmendamento_ParteEM(item.value, "Il titolo");
}

function NCapo_OnChange(item) {
    TestoEmendamento_ParteEM(item.value, "Il capo");
}

function TestoEmendamento_ParteEM(value, text) {
    if ($("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce").text().length < 200) {
        var tipoEMList = $('input[name="Emendamento.IDTipo_EM"]');
        $.each(tipoEMList,
            function(index, itemTipoEM) {
                if (itemTipoEM.checked) {
                    var testo = text + " " + value;
                    if (itemTipoEM.value == 1) {
                        $("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce")
                            .text(testo + " è soppresso.");
                    } else if (itemTipoEM.value == 2) {
                        $("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce")
                            .text(testo + " è modificato come segue: ");
                    } else if (itemTipoEM.value == 3) {
                        $("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce")
                            .text(testo + " viene emendato aggiungendo: ");
                    } else if (itemTipoEM.value == 4) {
                        $("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce")
                            .text(testo + " è sostituito.");
                    }
                }
            });
    }
}

function TestoEmendamento_TipoEM(tipo) {
    if ($("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce").text().length < 200) {
        var parteEMList = $('input[name="Emendamento.IDParte"]');
        $.each(parteEMList,
            function(index, itemParteEM) {
                if (itemParteEM.checked) {
                    var testo = "";
                    if (itemParteEM.value == 1) {
                        testo = "Il titolo del PDL ";
                    } else if (itemParteEM.value == 2) {
                        testo = "Il titolo " + $("#Emendamento_NTitolo").val();
                    } else if (itemParteEM.value == 3) {
                        testo = "Il capo " + $("#Emendamento_NCapo").val();
                    } else if (itemParteEM.value == 4) {
                        if ($("#pnlLettereOLD").is(":visible") && $("#Emendamento_NLettera").val() != "") {
                            testo = "La lettera " + $("#Emendamento_NLettera").val();
                        } else if ($("#pnlLettere").is(":visible") && $("#LettereList option:selected").val() != 0) {
                            testo = "La lettera " + $("#LettereList option:selected").text();
                        }

                        if ($("#pnlCommi").is(":visible") && $("#CommiList option:selected").val() != 0) {
                            if (testo == "") {
                                testo = "Il comma ";
                            } else {
                                testo = testo + " del comma ";
                            }

                            testo = testo + $("#CommiList option:selected").text();
                        }
                        if ($("#pnlArticoli").is(":visible") && $("#ArticoliList option:selected").val() != 0) {
                            if (testo == "") {
                                testo = "L'articolo ";
                            } else {
                                testo = testo + " dell'articolo ";
                            }

                            testo = testo + $("#ArticoliList option:selected").text();
                        }
                    } else if (itemParteEM.value == 5) {
                        testo = "La missione " +
                            $("#MissioniList option:selected").val() +
                            " programma " +
                            $("#Emendamento_NProgramma").val() +
                            " titolo " +
                            $("#TitoliMissioniList option:selected").val() +
                            " ";
                    }

                    if (tipo == 1) {
                        testo = testo + " è soppresso.";
                    } else if (tipo == 2) {
                        testo = testo + " è modificato come segue: ";
                    } else if (tipo == 3) {
                        testo = testo + " viene emendato aggiungendo: ";
                    } else if (tipo == 4) {
                        testo = testo + " è sostituito.";
                    }
                    $("#Emendamento_TestoEM_originale_ifr").contents().find("#tinymce").text(testo);
                }
            });
    }
}

function ClearMetaDati() {
    $("#pnlTitolo").hide();
    $("#pnlCapo").hide();
    $("#pnlMissioni").hide();
    $("#pnlArticoli").hide();
    $("#pnlCommi").hide();
    $("#pnlLettere").hide();
    $("#pnlLettereOLD").hide();

    $('input[id="Emendamento_IDTipo_EM"]').each(function(index, item) {
        if (item.checked) {
            item.checked = false;
        }
    });

    $("#Emendamento_NLettera").val("");
    $("#Emendamento_NCapo").val("");
    $("#Emendamento_NTitolo").val("");
    $("#Emendamento_NProgramma").val("");

    $("#ArticoliList").prop("selectedIndex", 0);
    var elemsArticoli = document.querySelectorAll("#ArticoliList");
    M.FormSelect.init(elemsArticoli, null);
    $("#CommiList").prop("selectedIndex", 0);
    var elemsCommi = document.querySelectorAll("#CommiList");
    M.FormSelect.init(elemsCommi, null);
    $("#LettereList").prop("selectedIndex", 0);
    var elemsLettere = document.querySelectorAll("#LettereList");
    M.FormSelect.init(elemsLettere, null);

    $("#MissioniList").prop("selectedIndex", 0);
    var elemsMissioni = document.querySelectorAll("#MissioniList");
    M.FormSelect.init(elemsMissioni, null);
    $("#TitoliMissioniList").prop("selectedIndex", 0);
    var elemsTitiliMissioni = document.querySelectorAll("#TitoliMissioniList");
    M.FormSelect.init(elemsTitiliMissioni, null);
}

function Articoli_OnChange(value, valueCommaSelected, valueLetteraSelected) {
    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/atti/commi",
            data: { id: value },
            type: "GET"
        }).done(function(result) {
            if (result.length > 0) {
                $("#pnlCommi").show();
                var commiSelect = $("#CommiList");
                commiSelect.empty();

                if (valueCommaSelected)
                    commiSelect.append('<option value="0">Seleziona comma</option>');
                else
                    commiSelect.append('<option selected="selected" value="0">Seleziona comma</option>');

                $.each(result,
                    function(index, item) {
                        var template = "";
                        if (item.UIDComma == valueCommaSelected)
                            template = "<option selected='selected'></option>";
                        else
                            template = "<option></option>";
                        commiSelect.append($(template).val(item.UIDComma).html(item.Comma));
                    });

                var elems = document.querySelectorAll("#CommiList");
                M.FormSelect.init(elems, null);

                if (valueLetteraSelected) {
                    Commi_OnChange(valueCommaSelected, valueLetteraSelected);
                }
            } else
                $("#pnlCommi").hide();

        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function Commi_OnChange(value, valueLetteraSelected) {
    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/atti/lettere",
            data: { id: value },
            type: "GET"
        }).done(function(result) {
            if (result.length > 0) {
                $("#pnlLettereOLD").hide();
                $("#pnlLettere").show();
                var lettereSelect = $("#LettereList");
                lettereSelect.empty();

                if (valueLetteraSelected)
                    lettereSelect.append('<option value="0">Seleziona lettera</option>');
                else
                    lettereSelect.append('<option selected="selected" value="0">Seleziona lettera</option>');

                $.each(result,
                    function(index, item) {
                        var template = "";
                        if (item.UIDLettera == valueLetteraSelected)
                            template = "<option selected='selected'></option>";
                        else
                            template = "<option></option>";
                        lettereSelect.append($(template).val(item.UIDLettera).html(item.Lettera));
                    });

                var elems = document.querySelectorAll("#LettereList");
                M.FormSelect.init(elems, null);
            } else {
                $("#pnlLettere").hide();

                var letteraOLD = $("#txtLetteraOLD").val();
                if (letteraOLD != null && letteraOLD != "")
                    $("#pnlLettereOLD").show();
            }

        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function EsportaXLS(attoUId) {
    swal("In che ordine devo esportare gli emendamenti?",
            {
                buttons: {
                    cancel: "Annulla",
                    presentazione: {
                        text: "Presentazione",
                        value: "0",
                    },
                    votazione: {
                        text: "Votazione",
                        value: "1",
                    }
                }
            })
        .then((value) => {

            if (value == null || value == "")
                return;

            go(baseUrl + "/emendamenti/esportaXLS?id=" + attoUId + "&ordine=" + value);
        });
}

function EsportaDOC(attoUId) {
    swal("In che ordine devo esportare gli emendamenti?",
            {
                buttons: {
                    cancel: "Annulla",
                    presentazione: {
                        text: "Presentazione",
                        value: "0",
                    },
                    votazione: {
                        text: "Votazione",
                        value: "1",
                    }
                }
            })
        .then((value) => {

            if (value == null || value == "")
                return;

            go(baseUrl + "/emendamenti/esportaDOC?id=" + attoUId + "&ordine=" + value);
        });
}

function DownloadStampa(stampaUId) {
    go(baseUrl + "/stampe/" + stampaUId);
}

function ResetStampa(stampaUId, url) {
    $.ajax({
        url: baseUrl + "/stampe/reset",
        data: { id: stampaUId },
        type: "GET"
    }).done(function(result) {
        console.log("RESET COMPLETE", result);
        go(url);
    }).fail(function(err) {
        console.log("error", err);
        ErrorAlert(err.message);
    });
}

function CambioStato(stato, descr) {
    var text = "";
    var listaEM = getListaEmendamenti();
    text = "Cambia stato di " + listaEM.length + " emendamenti in " + descr;

    swal(text,
            {
                buttons: { cancel: "Annulla", confirm: "Ok" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            var obj = {};
            obj.Stato = stato;
            obj.ListaEmendamenti = listaEM;

            $.ajax({
                url: baseUrl + "/emendamenti/modifica-stato",
                type: "POST",
                data: JSON.stringify(obj),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    go(data);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function GetPersoneFromDB() {
    var personeInSession = getListaPersone();
    if (personeInSession.length > 0)
        return personeInSession;
    return new Promise(function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/persone/all",
            type: "GET"
        }).done(function(result) {
            setListaPersone(result);
            resolve(result);
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function GetPersonePerInviti(attoUId, tipo) {
    return new Promise(function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/notifiche/destinatari?atto=" + attoUId + "&tipo=" + tipo,
            type: "GET"
        }).done(function(result) {
            resolve(result);
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

//SEGRETERIA
function Ordina_EMTrattazione(attoUId) {
    $.ajax({
        url: baseUrl + "/emendamenti/ordina?id=" + attoUId,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function(data) {
        if (data.message) {
            ErrorAlert(data.message);
        } else {
            go(data);
        }
    }).fail(function(err) {
        console.log("error", err);
        ErrorAlert(err.message);
    });
}

function SpostaUP_EMTrattazione() {
    var selezionaTutti = getSelezionaTutti();
    var listaEM = getListaEmendamenti();
    if (listaEM.length > 1 || selezionaTutti) {
        ErrorAlert("Puoi spostare solo 1 emendamento alla volta");
        return;
    }
    if (listaEM.length == 0) {
        ErrorAlert("Seleziona 1 emendamento da spostare");
        return;
    }

    $.ajax({
        url: baseUrl + "/emendamenti/ordina-up?id=" + listaEM[0],
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function(data) {
        if (data.message) {
            ErrorAlert(data.message);
        } else {
            go(data);
        }
    }).fail(function(err) {
        console.log("error", err);
        ErrorAlert(err.message);
    });
}

function SpostaDOWN_EMTrattazione() {
    var selezionaTutti = getSelezionaTutti();
    var listaEM = getListaEmendamenti();
    if (listaEM.length > 1 || selezionaTutti) {
        ErrorAlert("Puoi spostare solo 1 emendamento alla volta");
        return;
    }
    if (listaEM.length == 0) {
        ErrorAlert("Seleziona 1 emendamento da spostare");
        return;
    }

    $.ajax({
        url: baseUrl + "/emendamenti/ordina-down?id=" + listaEM[0],
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function(data) {
        if (data.message) {
            ErrorAlert(data.message);
        } else {
            go(data);
        }
    }).fail(function(err) {
        console.log("error", err);
        ErrorAlert(err.message);
    });
}

function Sposta_EMTrattazione() {
    var selezionaTutti = getSelezionaTutti();
    var listaEM = getListaEmendamenti();
    if (listaEM.length > 1 || selezionaTutti) {
        ErrorAlert("Puoi spostare solo 1 emendamento alla volta");
        return;
    }
    if (listaEM.length == 0) {
        ErrorAlert("Seleziona 1 emendamento da spostare");
        return;
    }
    swal("Sposta emendamento selezionato in una posizione precisa",
            {
                content: {
                    element: "input",
                    attributes: { type: "number" }
                },
                buttons: { cancel: "Annulla", confirm: "Ok" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/emendamenti/sposta?id=" + listaEM[0] + "&pos=" + value,
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    go(data);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function Proponenti_OnChange(tipo) {
    var pnlConsiglieri = $("#pnlProponentiConsiglieri");
    var pnlAssessori = $("#pnlProponentiAssessori");
    if (tipo == 1) {
        //CONSIGLIERI
        pnlAssessori.hide();
        pnlConsiglieri.show();
    } else if (tipo == 2) {
        pnlConsiglieri.hide();
        pnlAssessori.show();
    }
}

//EVENTI ATTI

function GetArticoliAtto(attoUId) {
    var tableCommi = $("#tableCommi");
    var tableLettere = $("#tableLettere");
    tableCommi.empty();
    tableLettere.empty();
    tableCommi.append("<li class='collection-item'>Crea da un articolo</li>");
    tableLettere.append("<li class='collection-item'>Crea da un comma</li>");

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/atti/articoli",
            data: { id: attoUId },
            type: "GET"
        }).done(function(result) {
            var table = $("#tableArticoli");
            table.empty();

            table.append('<li class="collection-item"><div>Nuovo articolo <a onclick="CreaArticolo(\'' +
                attoUId +
                '\')" class="secondary-content blue-text"><i class="material-icons">add</i></a></div></li>');

            if (result.length > 0) {
                $.each(result,
                    function(index, item) {
                        table.append('<li onclick="GetCommiArticolo(\'' +
                            item.UIDArticolo +
                            '\')" class="collection-item" uid="' +
                            item.UIDArticolo +
                            '"><div>' +
                            item.Articolo +
                            " " +
                            '<a onclick="EliminaArticolo(\'' +
                            item.UIDArticolo +
                            '\')" class="secondary-content"><i class="material-icons red-text">delete</i></a>' +
                            "</div></li>");
                    });
            }
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function GetCommiArticolo(articoloUId) {

    $("#tableArticoli").find("li").removeClass("active");
    $("#tableArticoli").find("li[uid='" + articoloUId + "']").addClass("active");

    var tableLettere = $("#tableLettere");
    tableLettere.empty();
    tableLettere.append("<li class='collection-item'>Crea da un comma</li>");

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/atti/commi",
            data: { id: articoloUId },
            type: "GET"
        }).done(function(result) {
            var table = $("#tableCommi");
            table.empty();

            table.append('<li class="collection-item"><div>Nuovo comma <a onclick="CreaComma(\'' +
                articoloUId +
                '\')" class="secondary-content blue-text"><i class="material-icons">add</i></a></div></li>');

            if (result.length > 0) {
                $.each(result,
                    function(index, item) {
                        table.append('<li onclick="GetLettereComma(\'' +
                            item.UIDComma +
                            '\')" class="collection-item" uid="' +
                            item.UIDComma +
                            '"><div>' +
                            item.Comma +
                            " " +
                            '<a onclick="EliminaComma(\'' +
                            item.UIDComma +
                            '\')" class="secondary-content"><i class="material-icons red-text">delete</i></a>' +
                            "</div></li>");
                    });
            }

        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function GetLettereComma(commaUId) {

    $("#tableCommi").find("li").removeClass("active");
    $("#tableCommi").find("li[uid='" + commaUId + "']").addClass("active");

    return new Promise(async function(resolve, reject) {
        $.ajax({
            url: baseUrl + "/atti/lettere",
            data: { id: commaUId },
            type: "GET"
        }).done(function(result) {
            var table = $("#tableLettere");
            table.empty();

            table.append('<li class="collection-item"><div>Nuova lettera <a onclick="CreaLettera(\'' +
                commaUId +
                '\')" class="secondary-content blue-text"><i class="material-icons">add</i></a></div></li>');

            if (result.length > 0) {
                $.each(result,
                    function(index, item) {
                        table.append("<li class='collection-item' uid='" +
                            item.UIDLettera +
                            "'><div>" +
                            item.Lettera +
                            " " +
                            '<a onclick="EliminaLettera(\'' +
                            item.UIDLettera +
                            '\')" class="secondary-content"><i class="material-icons red-text">delete</i></a>' +
                            "</div></li>");
                    });
            }
        }).fail(function(err) {
            console.log("error", err);
            ErrorAlert(err.message);
        });
    });
}

function CreaArticolo(attoUId) {

    swal("Inserisci articolo",
            {
                content: { element: "input", attributes: { placeholder: "1-5, 10, 22-34" } },
                buttons: { cancel: "Annulla", confirm: "Aggiungi" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/atti/crea-articoli?id=" + attoUId + "&articoli=" + value,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    GetArticoliAtto(attoUId);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function CreaComma(articoloUId) {

    swal("Inserisci comma",
            {
                content: { element: "input", attributes: { placeholder: "1-5, 10, 22-34" } },
                buttons: { cancel: "Annulla", confirm: "Aggiungi" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/atti/crea-commi?id=" + articoloUId + "&commi=" + value,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    GetCommiArticolo(articoloUId);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function CreaLettera(commaUId) {

    swal("Inserisci lettera",
            {
                content: { element: "input", attributes: { placeholder: "1-5, 10, 22-34" } },
                buttons: { cancel: "Annulla", confirm: "Aggiungi" }
            })
        .then((value) => {
            if (value == null || value == "")
                return;

            $.ajax({
                url: baseUrl + "/atti/crea-lettere?id=" + commaUId + "&lettere=" + value,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    GetLettereComma(commaUId);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function EliminaArticolo(articoloUId) {
    swal({
            title: "Sei sicuro?",
            text: "Verranno eliminati tutti i commi e le loro lettere associate",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
        .then((willDelete) => {
            if (!willDelete) return;

            $.ajax({
                url: baseUrl + "/atti/elimina-articolo?id=" + articoloUId,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    GetArticoliAtto($("#Atto_UIDAtto").val());
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function EliminaComma(commaUId) {
    swal({
            title: "Sei sicuro?",
            text: "Verranno eliminate anche tutte le lettere associate",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
        .then((willDelete) => {
            if (!willDelete) return;

            $.ajax({
                url: baseUrl + "/atti/elimina-comma?id=" + commaUId,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    var uidArticoloAttivo = $("#tableArticoli").find("li.active").attr("uid");
                    GetCommiArticolo(uidArticoloAttivo);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function EliminaLettera(letteraUId) {
    swal({
            title: "Sei sicuro?",
            text: "Verrà eliminata la lettera selezionata",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
        .then((willDelete) => {
            if (!willDelete) return;

            $.ajax({
                url: baseUrl + "/atti/elimina-lettera?id=" + letteraUId,
                method: "GET"
            }).done(function(data) {
                if (data.message) {
                    ErrorAlert(data.message);
                } else {
                    var uidCommaAttivo = $("#tableCommi").find("li.active").attr("uid");
                    GetLettereComma(uidCommaAttivo);
                }
            }).fail(function(err) {
                console.log("error", err);
                ErrorAlert(err.message);
            });
        });
}

function PubblicaFascicolo(attoUId, ordine) {
    var obj = {};
    obj.Id = attoUId;
    obj.Ordinamento = ordine;


    if (ordine == 1) {
        obj.Abilita = $("#chkAbilitaFascicoloPresentazione_" + attoUId)[0].checked;
    } else {
        obj.Abilita = $("#chkAbilitaFascicoloVotazione_" + attoUId)[0].checked;
    }

    $.ajax({
        url: baseUrl + "/atti/abilita-fascicolazione",
        type: "POST",
        data: JSON.stringify(obj),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function(data) {
        if (data.message) {
            ErrorAlert(data.message);
        }
        SuccessAlert("Fascicolo pubblicato", "");
    }).fail(function(err) {
        console.log("error", err);
        ErrorAlert(err.message);
    });
}

//NOTIFICHE

function GetDestinatariNotifica(notificaId) {
    var panel = $("#pnlDestinatariNotifica_" + notificaId);
    panel.empty();
    $.ajax({
        url: baseUrl + "/notifiche/" + notificaId + "/destinatari",
        type: "GET",
    }).done(function(data) {
        panel.append(data);
    }).fail(function(err) {
        console.log("error", err);
        ErrorAlert(err.message);
    });
}

// NOTIFICATION SWEETALERT.JS

function SuccessAlert(message) {
    swal({
        title: "Bel lavoro!",
        text: message,
        icon: "success",
        button: "OK"
    });
}

function SuccessAlert(message, url) {
    swal({
        title: "Bel lavoro!",
        text: message,
        icon: "success",
        button: "OK"
    }).then((value) => {
        if (value == null || value == "")
            return;
        go(url);
    });
}

function ErrorAlert(message) {
    //var body = JSON.parse(message);
    swal({
        title: "Attenzione!",
        text: message,
        icon: "error",
        button: "Ooops!"
    });
}

$.fn.serializeObject = function() {
    var o = {};
    var a = this.serializeArray();
    $.each(a,
        function() {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || "");
            } else {
                o[this.name] = this.value || "";
            }
        });
    return o;
};