   M S B A M L   `   `   `  ����=  9PM, Version=1.0.1.9, Culture=neutral, PublicKeyToken=nullclr-namespace:PMPM  5   	       PM.PM_ModelForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   "localclr-namespace:PM     5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   $��13(�5   	   $��Arial)�5	   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ PM_ModelForm��5   	   $	��500��6   $	��700��6+   .��5      � ���#008082 � RowDefinitions 5      �� 5   
   $��1* �6   �� 5   
   $��3* �6   �� 5   
   $��4* �6   �� 5   
   $��1* �6   .+�5      � $��0��6   ���#FFF � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      .+�5   
   � $��0��6   $��5q�6   ���#008082.+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�62   $��Center=�6M   .+�5      '  !MaterialDesignThemes.Wpf.PackIcon  ���#FFF$��25��6<   $��25��6H   �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=null]]   Kind$ Outbox=�6S   �� 5      $
��Bold"�6   ���#FFF$��Arial)�6<   $��13(�6O   .��6]   Go Top6e   5      5       � 5!   
   $��1��6   $��5q�6   ���#008082.+�5"      �� $ 
Horizontal=�6   $��Center=�62   $��Center=�6M   .+�5#        ���#FFF$��25��6<   $��25��6H   $ Upload=�6S   �� 5$      $
��Bold"�6   ���#FFF$��Arial)�6<   $��13(�6O   .��6]   Go Up6d   5%      5&      � 5'   
   $��2��6   $��5q�6   ���#008082.+�5(      �� $ 
Horizontal=�6   $��Center=�62   $��Center=�6M   .+�5)        ���#FFF$��25��6<   $��25��6H   $ AccountSearch=�6S   �� 5*      $
��Bold"�6   ���#FFF$��Arial)�6<   $��13(�6O   .��6]   Query6d   5+      5,      5-      � 5.      $��1��6   .+�5/   
   &   System.Windows.Controls.DataGrid  $��3q�6   51      52      � 53      $��2��6    54   
   �� 55      $��2* �6   �� 56      $��3* �6   .+�58   
   � $��0��6    59      �� 5:      $��1* �6    �� 5;      $��5* �6    .+�5=      � $��0��6   .+�5>      �� $��Work In Process (WIP)��6   $��Center=�68   $��Center=�6S   
�������6p   5?      � 5@      $��1��6   .+�5A        $��3q�6   5C      5D      5E      � 5F   
   $��1��6   .+�5G        $��3q�6   5I      5J      5K      � 5L      $��3��6    5M   
   �� 5N      $��5* �6   �� 5O      $��1* �6   .+�5Q   
   � $��0��6   .+�5R      �� $��MsgPanel��6   $��Center=�6'   $��Center=�6B   
�������6_   5S      � 5T   
   $��1��6   .+�5U      �� $��	TimePanel��6   $��Center=�6(   $��Center=�6C   
�������6`   5V      5W      5X      5Y      