   M S B A M L   `   `   `  ����H  DPACK_PALT_NEW, Version=1.0.4.8, Culture=neutral, PublicKeyToken=null%clr-namespace:PACK_PALT	PACK_PALT  5   	       PACK_PALT.Setup_Station   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:PACK_PALT     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5	   	   -   5        $��setup_station��5   N      MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	    � Title$ Setup_Station��5   	   $	��450��6   $	��400��6,    � WindowState$ Normal=�68    � WindowStartupLocation$ CenterScreen=�6c   $y�Visible=�6�   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��47* �6   �� 5      $��2* �6   ��5   
   �� 5       �� EndPoint$ 	0.504,1.5&�6"    �� 
StartPoint$ 
0.504,0.03&�67   .��5      �  � Color$ #03405D��6   	 � Offset$	 0Q�6/   � 5      $ #0AABCF��6   $	 0.567Q�6/   5      
 � RowDefinitions
 5   
   �� 5      $
��auto �6   6,   �� 5       $��2* �6   6*   �� 5!      $
��2.5* �6   6,   �� 5"      6   .+�5$   
   � $��0��6   $��2��6    5%      �� 5&      $
��0.5* �6#   62   �� 5'      $��1* �6#   60   �� 5(      $��2* �6#   60   .+�5*      � $��0��6   $��Center=�6#   .+�5+      �� -   5         ��Name$7 1MaterialDesignFloatingHintComboBoxEnabledComboBox��5,       d� 	IsChecked$
 TrueY�5-      $��10q�6&   5.      5/      � 50      $��1��6   .+�51      �� 
�������   MaterialDesignBody2TextBlock#��[ 52       �� TextAlignment$ Center=�6K   $��0 11 0 0q�6b   Change Line54      55      � 56      $��2��6   $��0 0 10 0q�6#   .+�57      �� -   5        $��cbb_Line_name��57   Q   $��14(�6   
���   �
������� �� 
IsEditable . {�58      ��  �� Path$ 	IsChecked� �� ElementName$7 1MaterialDesignFloatingHintComboBoxEnabledComboBox��5:      5;      5<      � 5=   
   $��1��6   $��2��6    5>      �� 5?      $��1* �6#   60   �� 5@      $��2* �6#   60   .+�5B      � $��0��6   
 5C      �� 5D      6&   �� 5E      6&   �� 5F      6&   .+�5H      � $��0��6   .+�5I      #  MaterialDesignThemes.Wpf.Chip  
�������.��6=   	Section5K      5L      � 5M      $��1��6   .+�5N        
�������.��6=   
Group name5P      5Q      � 5R      $��2��6   .+�5S        
�������.��6=   Station name5U      5V      5W      � 5X      $��1��6   
 5Y      �� 5Z      6&   �� 5[      6&   �� 5\      6&   .+�5^      � $��0��6   $��Center=�6$   .+�5_      #  MaterialDesignThemes.Wpf.Card  $��10 0 5 0q�6*   .��5`      �� -   5         ��Name$ txt_section��5`   "   {�.  ;  5MaterialDesignThemes.Wpf.Transitions.TransitionAssist   DisableTransitions . $��30��6�   ��67   �� $ 	dgr_group��$ SelectedItem.section_name�6�   5a      5b      � 5c      $��1��6   .+�5d        $��10 0 5 0q�6*   .��5e      �� -   5        $ txt_group_name��5e   "   {�.   . $��30��6�   ��6:   �� $ 	dgr_group��$ SelectedItem.group_name�6�   5f      5g      � 5h      $��2��6   .+�5i        $��10 0 5 0q�6*   .��5j      �� -   5        $ txt_station_name��5j   "   {�.   . $��30��6�   ��6<   �� $ 	dgr_group��$ SelectedItem.station_name�6�   5k      5l      5m      5n      � 5o   
   $��2��6   $��2��6   .+�5r      &   System.Windows.Controls.DataGrid  -   5          Name$ 	dgr_group��5r   )   $��8 8 8 0q�6      
IsReadOnly . 6Q   5s      � 5t   
   $��3��6   $��2��6    5u      �� 5v      6%   �� 5w      6%   .+�5y      � $��0��6   .+�5z      �� -   5        $��
ok_station��5z       $  MaterialDesignRaisedAccentButton#��[ 5{      $	��100��5|       � ToolTip$5 /Resource name: MaterialDesignRaisedAccentButton��5}      .��6d   OK5      5�      � 5�      $��1��6   .+�5�      �� -	   5        $��Cancel_station��5�       # MaterialDesignRaisedLightButton#��[ 5�      $	��100��5�      $4 .Resource name: MaterialDesignRaisedLightButton��5�      .��6g   Cancel5�      5�      5�      5�      5�      