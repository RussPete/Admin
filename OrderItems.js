// OrderItems.js
// Copyright (c) 2019 PeteSoft, LLC. All Rights Reverved

$(document).ready(function ()
{
    // make the Descriptions sortable
    $("ul.Items").sortable(
        {
            connectWith: "ul.Items",
            stop: SetSeq
        }).disableSelection();

    // hide the item label and show the edit text box
    $("ul.Items").on("click", "li", function ()
    {
        var lbl = $(this).find("label");
        lbl.hide();
        lbl.next().show().focus();
    });

    // hide the edit text box and show the label
    $("ul.Items").on("focusout", "li>input[type=text]", function ()
    {
        $(this).hide();
        $(this).prev().text($(this).val());
        $(this).prev().show();
    });

    // add a new item
    $("#btnNew").click(function ()
    {
        var NumItems = $("#hfNumItems").val();

        var Str = NewHtml.replace(/xxxx/g, NumItems).replace(/New Category/, "");
        $("#ulItems, #ulItems2").append(Str);

        NumItems++;
        $("#hfNumItems").val(NumItems);

        SetSeq();

        // make the new one an active text box
        $("#ulItems li, #ulItems2 li").last().find("label").click();
    });

    // hide items on the list
    $("ul.Items").on("click", "li>img.NotShow", function ()
    {
        SetDirty();
        var li = $(this).parent();
        li.addClass("NotShown");
        li.find("input[id^='hfDelete']").val(true);
        return false;
    });

    // show items on the List
    $("ul.Items").on("click", "li>img.Show", function ()
    {
        SetDirty();
        var li = $(this).parent();
        li.removeClass("NotShown");
        li.find("input[id^='hfDelete']").val(false);
        return false;
    });

    $(window).bind("beforeunload", function () 
    {
        return CheckDirty();
    });

    $("#btnUpdate").click(SkipDirty);
});

function SetSeq()
{
    SetDirty();
    $("ul.Items input.Seq").each(function (Index, Element)
    {
        $(this).val(Index);
    });
    if ($("ul.Items").length > 1)
    {
        BalanceLists();
    }
}

function BalanceLists()
{
    var c1 = $("#ulItems1 li").length;
    var c2 = $("#ulItems2 li").length;

    while (c1 < c2 || c1 > c2 + 1)
    {
        if (c1 < c2)
        {
            // shift from 2 to 1
            $("#ulItems1").append($("#ulItems2 li").first().detach());
        }
        else
        {
            // shift from 1 to 2
            $("#ulItems2").prepend($("#ulItems1 li").last().detach());
        }

        c1 = $("#ulItems1 li").length;
        c2 = $("#ulItems2 li").length;
    }
}
