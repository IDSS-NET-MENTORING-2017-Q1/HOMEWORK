function refresh() {
    $("#loader-container").css("display", "block");
    $.get("/Images/Get", {
        searchPhrase: $("#search-phrase").val()
    }).done(function (data) {
        $("#results").html(data);
        $("#loader-container").css("display", "none");
    });
}

$(document).ready(function () {
    refresh();
});