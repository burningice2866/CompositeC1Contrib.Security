<%@ Control Language="C#" AutoEventWireup="true" Inherits="CompositeC1Contrib.Security.Web.UI.InheritedPermissions" %>

<asp:Repeater ID="rptAllowedRoles" Visible="false" runat="server">
    <HeaderTemplate>
        Allowed roles
                <ul>
    </HeaderTemplate>

    <ItemTemplate>
        <li><%# Container.DataItem %></li>
    </ItemTemplate>

    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>

<asp:Repeater ID="rptDeniedRoles" Visible="false" runat="server">
    <HeaderTemplate>
        Denied roles
                <ul>
    </HeaderTemplate>

    <ItemTemplate>
        <li><%# Container.DataItem %></li>
    </ItemTemplate>

    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>

<asp:PlaceHolder ID="plcNoInheritance" Visible="false" runat="server">
    This item has no inherited permissions
</asp:PlaceHolder>
