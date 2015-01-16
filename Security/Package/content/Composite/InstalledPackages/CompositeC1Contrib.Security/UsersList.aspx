<?xml version="1.0" encoding="UTF-8" ?>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="CompositeC1Contrib.Security.Web.UI.UsersListPage" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
    <control:httpheaders runat="server" />

    <head runat="server">
        <title>Users list</title>

        <control:styleloader runat="server" />
        <control:scriptloader type="sub" runat="server" />

        <script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-1.9.0.js"></script>
        
        <script src="/Composite/InstalledPackages/CompositeC1Contrib.Security/bindings/UserCommandBinding.js"></script>
        
        <link href="/Composite/InstalledPackages/CompositeC1Contrib.Security/list.css" rel="stylesheet" />
        
        <script type="text/javascript">
            function edit(username) {
                $.ajax({
                    type: 'POST',
                    url: '<%= Request.Url.LocalPath %>/InvokeEditUserWorkflow',
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: "{ username: '"+ username +"', consoleId: '<%= ConsoleId %>' }",

                    success: function() {
                        MessageQueue.update();
                    }
                });
            }
        </script>
    </head>

    <body
        id="root"
        data-consoleid="<%= ConsoleId %>"
        data-type="<%= Type %>">
        <form runat="server" class="updateform updatezone">
            <ui:broadcasterset>
                <ui:broadcaster id="broadcasterHasSelection" isdisabled="true" />
            </ui:broadcasterset>

            <ui:popupset></ui:popupset>

            <ui:page id="usersList" image="${icon:report}">
                <ui:toolbar id="toolbar">
                    <ui:toolbarbody>
                        <ui:toolbargroup>
                            <aspui:toolbarbutton autopostback="true" text="Refresh" imageurl="${icon:refresh}" runat="server" onclick="OnRefresh" />
                        </ui:toolbargroup>
                    </ui:toolbarbody>
                </ui:toolbar>

                <ui:flexbox id="flexbox">
                    <ui:scrollbox id="scrollbox">
                        <table id="filter">
                            <thead>
                                <tr>
                                    <th>Email</th>
                                    <th class="input">
                                        <div>
                                            <aspui:textbox runat="server" id="txtEmail" width="100" />
                                        </div>
                                    </th>

                                    <th>Username</th>
                                    <th class="input">
                                        <div>
                                            <aspui:textbox runat="server" id="txtUsername" width="100" />
                                        </div>
                                    </th>
                                
                                    <th width="100%">
                                        <aspui:ClickButton autopostback="true" text="Filter" runat="server" onclick="OnFilter" />
                                    </th>
                                </tr>
                            </thead>
                        </table>

                        <asp:Repeater ID="rptUsers" ItemType="System.Web.Security.MembershipUser" runat="server">
                            <HeaderTemplate>
                                <table width="100%" id="logtable">
                                    <thead>
                                        <tr>
                                            <th>
                                                <ui:text label="Username" />
                                            </th>

                                            <th>
                                                <ui:text label="Email" />
                                            </th>
                                            
                                            <th style="width: 50px"></th>
                                        
                                            <th width="100%"></th>
                                        </tr>
                                    </thead>

                                    <tbody>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <ui:labelbox label="<%# Server.HtmlEncode(Item.UserName) %>" />
                                    </td>

                                    <td>
                                        <ui:labelbox label="<%# Server.HtmlEncode(Item.Email) %>" />
                                    </td>
                                    
                                    <td class="command">
                                        <a href="#" onclick="edit('<%# Item.UserName %>')">Edit</a>
                                    </td>
                                </tr>
                            </ItemTemplate>

                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </ui:scrollbox>

                    <ui:toolbar id="bottomtoolbar">
                        <ui:toolbarbody>
                            <ui:toolbargroup>
                                <aspui:selector runat="server" id="PageSize" autopostback="true" onselectedindexchanged="OnRefresh">
                                    <asp:ListItem>25</asp:ListItem>
                                    <asp:ListItem>100</asp:ListItem>
                                    <asp:ListItem>500</asp:ListItem>
                                </aspui:selector>
                            </ui:toolbargroup>

                            <ui:toolbargroup>
                                <aspui:toolbarbutton runat="server"
                                    id="PrevPage"
                                    client_tooltip="Previous"
                                    client_image="${icon:previous}"
                                    client_image-disabled="${icon:previous-disabled}"
                                    onclick="Prev" />

                                <aspui:textbox runat="server" readonly="True" id="PageNumber" width="20" />

                                <aspui:toolbarbutton runat="server"
                                    id="NextPage"
                                    client_tooltip="Next"
                                    client_image="${icon:next}"
                                    client_image-disabled="${icon:next-disabled}"
                                    onclick="Next" />
                            </ui:toolbargroup>
                        </ui:toolbarbody>
                    </ui:toolbar>

                    <ui:cover id="cover" hidden="true" />
                </ui:flexbox>
            </ui:page>
        </form>
    </body>
</html>
