/**
* Main JS file.
* 
* @class Window
* @module General
*/

var refreshVMListIntervalID = 0, refreshSitesListIntervalID = 0;

/**
* Document onload, call to initialize plugins and eventhandlers.
* 
* @method document.onload
*/
$(function () {
    Waves.attach(".btn");
    Waves.init();

    $("a, button").vibrate();

    if ($("#virtual-machines:visible").length > 0) {
        refreshVMListIntervalID = window.setInterval(refreshVMList, refreshInterval * 1000);

        $("#virtual-machines").on("click", ".fa-power-off", toggleVM);
        $("#virtual-machines .panel-heading button").click(refreshVMList);

        $('#virtual-machines table').addSortWidget({
            img_asc: baseUrl + "Content/Images/Sorttable/asc_sort.gif",
            img_desc: baseUrl + "Content/Images/Sorttable/desc_sort.gif",
            img_nosort: baseUrl + "Content/Images/Sorttable/no_sort.gif"
        });
    }

    if ($("#sites:visible").length > 0) {
        refreshSitesListIntervalID = window.setInterval(refreshSites, refreshInterval * 1000);

        $("#sites .panel-heading button").click(refreshSites);
        $("#sites").on("click", "button.fa-copy:visible", copyPath);
        $("#sites").on("click", "button.fa-power-off:visible", toggleSite);

        $('#sites table').addSortWidget({
            img_asc: baseUrl + "Content/Images/Sorttable/asc_sort.gif",
            img_desc: baseUrl + "Content/Images/Sorttable/desc_sort.gif",
            img_nosort: baseUrl + "Content/Images/Sorttable/no_sort.gif"
        });
    }

    initializeFancybox();
});

/**
 * Initializes and removes fancybox click handlers for application modal dialog.
 * Called on load and when refreshing site list.
 * 
 * @method initializeFancybox
 */
function initializeFancybox() {
    $(".fa-puzzle-piece").off().on("click", function () {
        $.fancybox.open({ src: $(this).data("href") }, {
            fullScreen: false,
            closeBtn: true,
            iframe: {
                css: { "max-width": "500px" }
            }
        });
    });
}

function openConfirmDialog(title, data, buttonClick) {
    $("div#confirm-dialog h2").html(title);

    $.each(data, function (index, value) {
        $.each(value, function (index, value) {
            $("div#confirm-dialog").data(index, value);
        });
    });

    $("div#confirm-dialog button").off().on("click", buttonClick);

    $.fancybox.open({
        src: "#confirm-dialog",
        opts: { closeBtn: false, closeClickOutside: false }
    });
}

/**
 * Refreshes the VM table with fresh data.
 * 
 * @method refreshVMList
 */
function refreshVMList() {
    $("#virtual-machines").isLoading();

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
                clone.find("button").addClass("btn-" + (value.State === 2 ? "success" : "danger"));
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

/**
 * Toggles the state of the VM to On/Off.
 * 
 * @method toggleVM
 */
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

/**
 * Refreshes the sites table with fresh data.
 * 
 * @method refreshSites
 */
function refreshSites() {
    $("#sites").isLoading();

    window.clearInterval(refreshSitesListIntervalID);
    var $this = $(this);

    $.getJSON({
        url: baseUrl + "Sites/GetSites",
        type: "POST",
        success: function (data) {
            $("#sites tbody tr:not(.hidden)").remove();

            $.each(data, function (index, value) {
                var clone = $("#sites tr.hidden").clone();

                clone.find(".fa-power-off").addClass("btn-" + (value.State === 1 ? "success" : "danger"));

                if (value.Applications.length) {
                    var anchor = clone.find(".fa-puzzle-piece");
                    var href = anchor.data("href");
                    
                    anchor.removeClass("disabled").data("href", href + "?sitename=" + value.Name);
                }

                $.each(value.Bindings, function (index, value) {
                    var binding = clone.find(".protocol.hidden").clone();
                    binding.removeClass("hidden").attr("href", value).html(index);
                    binding.appendTo(clone.find(".bindings"));
                });

                clone.find(".protocol.hidden").remove();
                clone.find("td.name").html(value.Name);
                clone.find("textarea").val(value.PhysicalPath);
                clone.find("td.path").html(value.PhysicalPath);
                clone.removeClass("hidden");

                clone.appendTo($("#sites tbody"));
            });

            initializeFancybox();
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

/**
 * Toggles the state of the site to On/Off.
 * 
 * @method toggleSite
 * @param {Object} event    The JS event, used to stop the bubble.
 */
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

/**
 * Copies the path of a site.
 * 
 * @method copyPath
 * @param {Object} event    The JS event, used to stop the bubble.
 */
function copyPath(event) {
    event.stopPropagation();

    $(this).next().select();
    document.execCommand("copy");

    flashAlert("Copied to clipboard!", "success");
}

/**
 * Fades out the alert after alertTimeout (in seconds).
 * 
 * @method fadeOutAlert
 */
function fadeOutAlert() {
    window.setTimeout(function () {
        $("div.alert").fadeOut("fast", function () {
            $("div.alert").removeClass("alert-success alert-danger");
        });
    }, alertTimeout * 1000);
}

/**
 * Displays an alert message (and fades it after).
 * 
 * @method flashAlert
 * @param {String} message  The message to display.
 * @param {String} type     The bootstrap type of alert.
 */
function flashAlert(message, type) {
    $("div.alert").addClass("alert-" + type);
    $("div.alert").html(message);
    $("div.alert").fadeIn("fast");

    fadeOutAlert();
}