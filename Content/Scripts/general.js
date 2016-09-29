var alertTimeout = 5;

$(function () {
    Waves.attach(".btn");
    Waves.init();

    $("a, button").vibrate();

    $("body").on("click", ".glyphicon-off", toggleVM);
    $(".panel-heading button").click(refreshVMList);
});

function refreshVMList() {
    $(".panel").isLoading({
        text: "Loading",
        position: "overlay"
    });
    var $this = $(this);

    $.getJSON({
        url: "Index/GetVMs",
        type: "POST",
        success: function (data) {
            $("tbody tr:not(.hidden)").remove();

            $.each(data, function (index, value) {
                var clone = $("tr.hidden").clone();

                clone.find("td:eq(0)").html(value.Name);
                clone.find("td:eq(1)").html(value.TimeOfLastStateChangeFormatted);
                clone.find("td:eq(2)").html(value.GetOnTimeFormatted);
                clone.find("td:eq(3)").html(value.CoresAmount);
                clone.find("td:eq(4)").html(value.MemoryTotal + " " + value.MemoryAllocationUnits);
                clone.find("td:eq(5)").html(value.MAC);
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
        }
    });
}

function toggleVM() {
    var $this = $(this);

    if (confirm("Are you sure you want to toggle this VM " + ($(this).hasClass("btn-danger") ? "on" : "off") + "?")) {
        $this.addClass("disabled");
        $this.blur();

        $.post({
            url: "Index/ToggleVMState",
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