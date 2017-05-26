function refresh() {
    $.get("/Images/Get", {
        searchPhrase: $("#search-phrase").val()
    }).done(function (data) {
        $("#results").html(data);
    });
}

$(document).ready(function () {
    refresh();
});