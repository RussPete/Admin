function SetupRecentJobs()
{
    Qtips(".bookmark");
    $(".JobOnBar")
        .button()
        .click(function () {
            var loc = window.location;
            var Job = $(this).data("job");
            var Parts = loc.pathname.split("/");
            Parts[Parts.length - 1] = "Job.aspx";
            window.location.assign(loc.protocol + "//" + loc.host + (Parts).join("/") + "?JobRno=" + Job);
            return false;
        });
    $("#JobToolbar").hide();
    $(".bookmark").click(function () {
        $("#JobToolbar").toggle();
    })
}
