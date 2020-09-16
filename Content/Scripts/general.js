/**
* Main JS file.
*
* @class Window
* @module General
*/

var refreshVMListIntervalID = 0, refreshSitesListIntervalID = 0;
var settings;

/**
* Document onload, call to initialize plugins and eventhandlers.
*
* @method document.onload
*/
$(function () {
    settings = {
        alertTimeout: $("body").data("alerttimeout"),
        refreshInterval: $("body").data("refreshinterval"),
        baseUrl: $("body").data("baseurl")
    };

    if ("serviceWorker" in navigator) {
        window.addEventListener("load", function () {
            navigator.serviceWorker.register(settings.baseUrl + "Session/ServiceWorker", { scope: settings.baseUrl }).then(function (_registration) {
                // Registration was successful
            }, function (_err) {
                // registration failed :(
            });
        });
    }

    $("a, button").vibrate();

    $("footer .fa-arrows-alt").click(function () {
        $(document).fullScreen(!$(document).fullScreen());
    });

    initializeHelpShortcut();

    if ($("#virtual-machines:visible").length > 0) {
        refreshVMListIntervalID = window.setInterval(refreshVMList, settings.refreshInterval * 1000);

        $("#virtual-machines").on("click", ".fa-power-off", toggleVM);
        $("#virtual-machines .card-header button").click(refreshVMList);

        $('#virtual-machines table').addSortWidget({
            img_asc: settings.baseUrl + "Content/Images/Sorttable/asc_sort.gif",
            img_desc: settings.baseUrl + "Content/Images/Sorttable/desc_sort.gif",
            img_nosort: settings.baseUrl + "Content/Images/Sorttable/no_sort.gif"
        });
    }

    if ($("#sites:visible").length > 0) {
        refreshSitesListIntervalID = window.setInterval(refreshSites, settings.refreshInterval * 1000);

        $("#sites .card-header button").click(refreshSites);
        $("#sites").on("click", "button.fa-copy:visible", copyPath);
        $("#sites").on("click", "button.fa-power-off:visible", toggleSite);

        $('#sites table').addSortWidget({
            img_asc: settings.baseUrl + "Content/Images/Sorttable/asc_sort.gif",
            img_desc: settings.baseUrl + "Content/Images/Sorttable/desc_sort.gif",
            img_nosort: settings.baseUrl + "Content/Images/Sorttable/no_sort.gif"
        });
    }

    initializeFancybox();
});

/**
* Initializes the event handling for F1 keyboard shortcut to open the docs.
*
* @method initializeHelpShortcut
*/
function initializeHelpShortcut() {
    //Fix for F1 shortcut in IE.
    window.onhelp = function () {
        return false;
    };

    $("body").keydown(function (event) {
        //keycode 112 === F1

        if (event.which === 112) {
            $("footer .fa-question")[0].click();
            event.preventDefault();
        }
    });
}

/**
 * Initializes and removes fancybox click handlers for application modal dialog.
 * Called on load and when refreshing site list.
 *
 * @method initializeFancybox
 */
function initializeFancybox() {
    $.fancybox.defaults.smallBtn = $.fancybox.defaults.fullScreen = $.fancybox.defaults.slideShow = false;

    $(".fa-puzzle-piece:not(.disabled)").off().on("click", function () {
        $.fancybox.open({ src: $(this).data("href"), type: "iframe" });
    });
}

/**
* Shows a confirm dialog with yes/no buttons
*
* @method openConfirmDialog
* @param {String} title        The title to set for the confirm dialog.
* @param {String} data         The data attributes to set on the confirm dialog, for later us.
* @param {Object} buttonClick  callback for clicking the confirm button.
*/
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
        url: settings.baseUrl + "VMs/GetVMs",
        type: "POST",
        success: function (data) {
            $("#virtual-machines tbody tr:not(.d-none)").remove();

            $.each(data, function (index, value) {
                var clone = $("#virtual-machines tr.d-none").clone();

                clone.find("td.name").html(value.Name);
                clone.find("td.last-state-change").html(value.TimeOfLastStateChangeFormatted);
                clone.find("td.on-time").html(value.GetOnTimeFormatted);
                clone.find("td.cores").html(value.CoresAmount);
                clone.find("td.memory").html(value.MemoryTotal + " " + value.MemoryAllocationUnits);
                clone.find("td.mac").html(value.MAC);
                clone.find("button").addClass("btn-" + (value.State === 2 ? "success" : "danger"));
                clone.removeClass("d-none");

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

            refreshVMListIntervalID = window.setInterval(refreshVMList, settings.refreshInterval * 1000);
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

    openConfirmDialog("Are you sure?", null, function (btn) {
        $.fancybox.close();

        if ($(this).attr("id") === "confirm-yes") {
            $this.addClass("disabled");
            $.post({
                url: settings.baseUrl + "VMs/ToggleState",
                data: {
                    vmName: $.trim($this.closest("tr").find("td.name").html()),
                    state: $this.hasClass("btn-danger") ? 2 : 3
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

                    $this = undefined;
                }
            });
        }
    });
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
        url: settings.baseUrl + "Sites/GetSites",
        type: "POST",
        success: function (data) {
            $("#sites tbody tr:not(.d-none)").remove();

            $.each(data, function (index, value) {
                var clone = $("#sites tr.d-none").clone();

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
                clone.removeClass("d-none");

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

            refreshSitesListIntervalID = window.setInterval(refreshSites, settings.refreshInterval * 1000);
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
    var $this = $(this);

    openConfirmDialog("Are you sure?", null, function (btn) {
        $.fancybox.close();

        if ($(this).attr("id") === "confirm-yes") {
            $this.closest("tr").find(".protocol").toggleClass("disabled");

            $.post({
                url: settings.baseUrl + "Sites/" + ($this.hasClass("btn-success") ? "StopSite" : "StartSite"),
                data: {
                    sitename: $.trim($this.closest("tr").find(".name").html())
                },
                success: function () {
                    $this.toggleClass("btn-success btn-danger");
                    $this.closest("tr").toggleClass("disabled");
                },
                complete: function () {
                    $this = undefined;
                }
            });
        }
    });
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
    }, settings.alertTimeout * 1000);
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