﻿

#pragma checksum "D:\Windows 8 Projects\LiveBoard\LiveBoard\RoomListPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E539BBDE7C5808D06FE054BCBBEDFAC8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LiveBoard
{
    partial class RoomListPage : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button CreateButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button RefreshButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button BackButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock NicknameTextBlock; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ListView RoomListView; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Primitives.Popup RoomCreatePopup; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Primitives.Popup EnterPasswordPopup; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.PasswordBox EnterPasswordBox; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button PasswordConfirmButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button PasswordClearButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button PasswordCancelButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox RoomTitleTextBox; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.PasswordBox RoomPasswordTextBox; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox RoomMaximumNumberTextBox; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button PopupCreateButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button PopupResetButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button PopupCancelButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///RoomListPage.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            CreateButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("CreateButton");
            RefreshButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("RefreshButton");
            BackButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("BackButton");
            NicknameTextBlock = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("NicknameTextBlock");
            RoomListView = (global::Windows.UI.Xaml.Controls.ListView)this.FindName("RoomListView");
            RoomCreatePopup = (global::Windows.UI.Xaml.Controls.Primitives.Popup)this.FindName("RoomCreatePopup");
            EnterPasswordPopup = (global::Windows.UI.Xaml.Controls.Primitives.Popup)this.FindName("EnterPasswordPopup");
            EnterPasswordBox = (global::Windows.UI.Xaml.Controls.PasswordBox)this.FindName("EnterPasswordBox");
            PasswordConfirmButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("PasswordConfirmButton");
            PasswordClearButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("PasswordClearButton");
            PasswordCancelButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("PasswordCancelButton");
            RoomTitleTextBox = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("RoomTitleTextBox");
            RoomPasswordTextBox = (global::Windows.UI.Xaml.Controls.PasswordBox)this.FindName("RoomPasswordTextBox");
            RoomMaximumNumberTextBox = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("RoomMaximumNumberTextBox");
            PopupCreateButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("PopupCreateButton");
            PopupResetButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("PopupResetButton");
            PopupCancelButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("PopupCancelButton");
        }
    }
}


