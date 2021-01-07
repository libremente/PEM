function getListaEmendamenti() {
    var session_raw = sessionStorage.getItem("listaEmendamenti");
    if (session_raw == null)
        return {}
    return JSON.parse(session_raw);
}

function setListaEmendamenti(lista) {
    sessionStorage.setItem("listaEmendamenti", JSON.stringify(lista));
}

function getSelezionaTutti() {
    var session_raw = sessionStorage.getItem("SelezionaTutti");
    return JSON.parse(session_raw);
}

function setSelezionaTutti(seleziona) {
    sessionStorage.setItem("SelezionaTutti", JSON.stringify(seleziona));
}

function getListaPersone() {
    var session_raw = sessionStorage.getItem("listaPersone");
    if (session_raw == null)
        return {}
    return JSON.parse(session_raw);
}

function setListaPersone(lista) {
    sessionStorage.setItem("listaPersone", JSON.stringify(lista));
}

function getClientMode() {
    var session_raw = sessionStorage.getItem("CLIENT_MODE");
    return JSON.parse(session_raw);
}

function setClientMode(mode) {
    sessionStorage.setItem("CLIENT_MODE", JSON.stringify(mode));
}

function get_ListaLegislature() {
    var session_raw = sessionStorage.getItem("ListaLegislature");
    if (session_raw == null)
        return {}
    return JSON.parse(session_raw);
}

function set_ListaLegislature(obj) {
    sessionStorage.setItem("ListaLegislature", JSON.stringify(obj));
}

/////////////////////////////////////////////////////////////
////////
////////            FILTRI
////////
/////////////////////////////////////////////////////////////

//SEDUTE
function get_Filtri_Sedute() {
    var session_raw = sessionStorage.getItem("Filtri_Sedute");
    if (session_raw == null)
        return {}
    return JSON.parse(session_raw);
}

function set_Filtri_Sedute(obj) {
    sessionStorage.setItem("Filtri_Sedute", JSON.stringify(obj));
}

//EM
function get_Filtri_EM() {
    var session_raw = sessionStorage.getItem("Filtri_EM");
    if (session_raw == null)
        return {}
    return JSON.parse(session_raw);
}

function set_Filtri_EM(obj) {
    sessionStorage.setItem("Filtri_EM", JSON.stringify(obj));
}