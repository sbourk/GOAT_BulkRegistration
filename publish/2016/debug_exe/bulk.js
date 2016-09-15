function ProcessDietaryInformation(restriction, prefix) {
    if (!restriction || restriction == "\r")
        return;

    $(":checkbox[id*=" + prefix + "]").click();
    $("textarea[name*=" + prefix + "][class*='qtype_textarea']").val(restriction);

    $("tr[id*=" + prefix + "][class*='survey_question hide_me'][data-attendee*=" + prefix + "]").toggle("hide_me");
}
function ClickMyEmployer(prefix) {
    $("input[value='My employer'][id*=" + prefix + "]").click();
}
function AgreeWaiver(prefix) {
    $("input[name*=" + prefix + "][id='waiver']").click();
}

function noError() {
    return true;
}

window.onerror = noError;