var refreshIntervalID = 0;

$(function () {
    Waves.attach(".btn");
    Waves.init();

    $("a, button").vibrate();

    $("body").on("click", ".glyphicon-off", toggleVM);
    $(".panel-heading button").click(refreshVMList);

    refreshIntervalID = window.setInterval(refreshVMList, refreshInterval * 1000);
});

function refreshVMList() {
    $(".panel").isLoading({
        text: "Loading",
        position: "overlay"
    });

    window.clearInterval(refreshIntervalID);
    var $this = $(this);

    $.getJSON({
        url: "VM/GetVMs",
        type: "POST",
        success: function (data) {
            $("tbody tr:not(.hidden)").remove();

            $.each(data, function (index, value) {
                var clone = $("tr.hidden").clone();

                clone.find("td:eq(1)").html(value.Name);
                clone.find("td:eq(2)").html(value.TimeOfLastStateChangeFormatted);
                clone.find("td:eq(3)").html(value.GetOnTimeFormatted);
                clone.find("td:eq(4)").html(value.CoresAmount);
                clone.find("td:eq(5)").html(value.MemoryTotal + " " + value.MemoryAllocationUnits);
                clone.find("td:eq(6)").html(value.MAC);
                clone.find("button").addClass("btn-" + (value.State == 2 ? "success" : "danger"));
                clone.removeClass("hidden cloneable");

                clone.appendTo($("tbody"));
            });

            flashAlert("VM list refreshed!", "success");
        },
        error: function () {
            flashAlert("Something went wrong refreshing list!", "danger");
        },
        complete: function () {
            $(".panel").isLoading("hide");
            $this.blur();

            refreshIntervalID = window.setInterval(refreshVMList, refreshInterval * 1000);
        }
    });
}

function toggleVM() {
    var $this = $(this);
    $this.blur();

    if (confirm("Are you sure you want to toggle this VM " + ($(this).hasClass("btn-danger") ? "on" : "off") + "?")) {
        $this.addClass("disabled");
        
        $.post({
            url: "VM/ToggleState",
            data: {
                vmName: $.trim($(this).closest("tr").find("td:eq(0)").html()),
                state: $(this).hasClass("btn-danger") ? 2 : 3
            },
            success: function (data) {
                $this.toggleClass("btn-success btn-danger disabled");

                flashAlert(data, "success");
            },
            error: function () {
                flashAlert("Something went wrong changing VM state!", "danger");
            },
        });
    }
}

function fadeOutAlert() {
    window.setTimeout(function () {
        $("div.alert").fadeOut("fast", function () {
            $("div.alert").removeClass("alert-success alert-danger");
        });
    }, alertTimeout * 1000);
}

function flashAlert(message, type) {
    $("div.alert").addClass("alert-" + type);
    $("div.alert").html(message);
    $("div.alert").fadeIn("fast");

    fadeOutAlert();
}