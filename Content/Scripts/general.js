﻿var refreshVMListIntervalID = 0, refreshSitesListIntervalID = 0;

$(function () {
    Waves.attach(".btn");
    Waves.init();

    $("a, button").vibrate();

    $("body").on("click", "#virtual-machines .glyphicon-off", toggleVM);
    $("#virtual-machines .panel-heading button").click(refreshVMList);
    $("#sites .panel-heading button").click(refreshSites);
    $("#sites").on("click", "button.copy-path", copyPath);
    $("#sites").on("click", "tr td:first-child button:visible", toggleSite);

    $('#virtual-machines table').addSortWidget({
        img_asc: baseUrl + "Content/Images/Sorttable/asc_sort.gif",
        img_desc: baseUrl + "Content/Images/Sorttable/desc_sort.gif",
        img_nosort: baseUrl + "Content/Images/Sorttable/no_sort.gif",
    });
    $('#sites table').addSortWidget({
        img_asc: baseUrl + "Content/Images/Sorttable/asc_sort.gif",
        img_desc: baseUrl + "Content/Images/Sorttable/desc_sort.gif",
        img_nosort: baseUrl + "Content/Images/Sorttable/no_sort.gif",
    });

    if ($("#virtual-machines:visible").length > 0) {
        refreshVMListIntervalID = window.setInterval(refreshVMList, refreshInterval * 1000);
    }

    if ($("#sites:visible").length > 0) {
        refreshSitesListIntervalID = window.setInterval(refreshSites, refreshInterval * 1000);
    }
});

function refreshVMList() {
    $("#virtual-machines").isLoading({
        text: "Loading",
        position: "overlay"
    });

    window.clearInterval(refreshVMListIntervalID);
    var $this = $(this);

    $.getJSON({
        url: baseUrl + "VMs/GetVMs",
        type: "POST",
        success: function (data) {
            $("#virtual-machines tbody tr:not(.hidden)").remove();

            $.each(data, function (index, value) {
                var clone = $("#virtual-machines tr.hidden").clone();

                clone.find("td:eq(1)").html(value.Name);
                clone.find("td:eq(2)").html(value.TimeOfLastStateChangeFormatted);
                clone.find("td:eq(3)").html(value.GetOnTimeFormatted);
                clone.find("td:eq(4)").html(value.CoresAmount);
                clone.find("td:eq(5)").html(value.MemoryTotal + " " + value.MemoryAllocationUnits);
                clone.find("td:eq(6)").html(value.MAC);
                clone.find("button").addClass("btn-" + (value.State == 2 ? "success" : "danger"));
                clone.removeClass("hidden");

                clone.appendTo($("#virtual-machines tbody"));
            });

            flashAlert("VM list refreshed!", "success");
        },
        error: function () {
            flashAlert("Something went wrong refreshing list!", "danger");
        },
        complete: function () {
            $("#virtual-machines").isLoading("hide");
            $this.blur();

            refreshVMListIntervalID = window.setInterval(refreshVMList, refreshInterval * 1000);
        }
    });
}

function toggleVM() {
    var $this = $(this);
    $this.blur();

    if (confirm("Are you sure you want to toggle this VM " + ($(this).hasClass("btn-danger") ? "on" : "off") + "?")) {
        $this.addClass("disabled");

        $.post({
            url: baseUrl + "VMs/ToggleState",
            data: {
                vmName: $.trim($(this).closest("tr").find("td:eq(1)").html()),
                state: $(this).hasClass("btn-danger") ? 2 : 3
            },
            success: function (data) {
                $this.toggleClass("btn-success btn-danger");

                flashAlert(data, "success");
            },
            error: function () {
                flashAlert("Something went wrong changing VM state!", "danger");
            },
            complete: function () {
                $this.toggleClass("disabled");
            }
        });
    }
}

function refreshSites() {
    $("#sites").isLoading({
        text: "Loading",
        position: "overlay"
    });

    window.clearInterval(refreshSitesListIntervalID);
    var $this = $(this);

    $.getJSON({
        url: baseUrl + "Sites/GetSites",
        type: "POST",
        success: function (data) {
            $("#sites tbody tr:not(.hidden)").remove();

            $.each(data, function (index, value) {
                var clone = $("#sites tr.hidden").clone();

                clone.find("td:eq(0) button").addClass("btn-" + (value.State == 1 ? "success" : "danger"));

                $.each(value.Bindings, function (index, value) {
                    var binding = clone.find("td:eq(0) a.protocol.hidden").clone()
                    binding.removeClass("hidden").attr("href", value).html(index);
                    binding.appendTo(clone.find("td:eq(0) .btn-group"));
                });

                clone.find("td:eq(1)").html(value.Name);
                clone.find("td:eq(2) textarea").val(value.PhysicalPath);
                clone.find("td:eq(2) button").html(value.PhysicalPath).attr("title", value.PhysicalPath);
                clone.removeClass("hidden");

                clone.appendTo($("#sites tbody"));
            });

            flashAlert("Site list refreshed!", "success");
        },
        error: function () {
            flashAlert("Something went wrong refreshing list!", "danger");
        },
        complete: function () {
            $("#sites").isLoading("hide");
            $this.blur();

            refreshSitesListIntervalID = window.setInterval(refreshSites, refreshInterval * 1000);
        }
    });
}

function toggleSite(event) {
    event.stopPropagation();

    if (confirm("Are you sure you want to " + ($(this).hasClass("btn-success") ? "stop" : "start") + " this website?")) {
        var button = $(this);

        $("tr").removeClass("active");
        $(this).closest("tr").find(".protocol").toggleClass("disabled");

        $.post({
            url: baseUrl + "Sites/" + ($(this).hasClass("btn-success") ? "StopSite" : "StartSite"),
            data: {
                sitename: $.trim($(this).closest("tr").find(".name").html())
            },
            success: function () {
                button.toggleClass("btn-success btn-danger");
                button.closest("tr").toggleClass("disabled");
            }
        });
    }
}

function copyPath(event) {
    event.stopPropagation();

    $(this).next().select();
    document.execCommand("copy");

    flashAlert("Copied to clipboard!", "success");
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