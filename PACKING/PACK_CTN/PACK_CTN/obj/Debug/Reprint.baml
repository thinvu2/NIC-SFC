   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.Reprint   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   2gif"http://wpfanimatedgif.codeplex.com     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��
FrmReprint��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FFE5F2F5 � Title$ Reprint��5   	   $	��400��6   $	��676��6&    � WindowState$ Normal=�62    � WindowStyle$
 None=�6G    � WindowStartupLocation$ CenterScreen=�6Z   
��� E�� � 	Resources 5       
 header �� Color  MenuItem  
Label_text  	Colortext  Icon��%�   &	       &	 !     &	 B     &	 c     &	 �     �� 5   
   $ #0889a6��6)   �� 5   
   $ #0889a6��6+   �� 5   
   $ #0889a6��6-   �� 5   
   $	 Red��6,   �� 5   
   $ #0889a6��6'   5      .��5      � $��0,0,2,0q�6    � RowDefinitions 5   
   �� 5       $
��auto �6   6,   �� 5!      $
��AUTO �6   6,   �� 5"      6   .+�5$   
   � $��0��6   .+�5%      *   $PACK_CTN.UserControlCTN.ControlBarUC  
	 � Tag	 6   �� *Title+
 �� ElementName$
 
FrmReprint��6N   5&      � 5'   
   $��1��6   #��[ 6   .+�5(      �� $��Reprint Label��6   $��Center=�6,   $��Center=�6G   $��Arial)�6z   $��24(�6�   
���������6d   Z� 6�   5)      � 5*   
   $��2��6   ��5+      �� 5,       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5-      �  � Color$ 	#FF36C6E6��6#    � Offset$ 0.975Q�65   � 5.      $ 	#FFC6E0E6��6#   $ 0.519Q�65   5/       � ColumnDefinitions 52      �� 53      $	��350 �6#   61   �� 54      $	��300 �6#   62   .+�56      � $��0��6    57      �� 58      $
��auto �6$   65   �� 59      6&   �� 5:      6&   �� 5;      6&   �� 5<      $
��auto �6$   64   .+�5?      � $��1��6    5@      �� 5A      $
��279* �6+   �� 5B      $	��71* �6+    5D      �� 5E      6*   �� 5F      6*   .+�5H      �� $��0��6!    �� Orientation$ 
Horizontal=�6.   $	��Top=�6G   $��
13,10,13,0q�6_   $��Center=�6s   $��2��6�   .+�5I      d� -   5        $��cbCartonLabel��5I   C   $
��Left=�6(    * &MaterialDesignSwitchAccentToggleButton#��[ 5J       � ToolTip$, &MaterialDesignSwitchAccentToggleButton��5K       d� 	IsChecked$ FalseY�5L      $��2��6-   6A   �� 5M      $��Carton Label��6    #��[ 68   $��5 0 0 0q�6c   {�6t   ��  �� Path$ 	IsChecked�$
 cbCartonLabel��6�   d� 5N      -   5        $��cbCustCartonLabel��5N   D   $
��Left=�6)   #��[ 5O      $, &MaterialDesignSwitchAccentToggleButton��5P      $ FalseY�5Q      $��2��6-   6A   �� 5R      $��Cust Carton Lable��6"   $��5 0 0 0q�6?   #��[ 6�   {�6P   �� $ 	IsChecked�$
 cbCustCartonLabel��6�   5S      �� 5T      $��1��6!   $ 
Horizontal=�6.   $	��Top=�6G   $��81,10,9.85,0q�6_   $��Center=�6u   .+�5U      �� $
��Left=�6!   $��	Lable QTY��6<   $��18(�6P   #��[ 6`   �� 5V      -   5        $��cbbLbQty��5V   #   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 1��63   $��18(�6V   $
��Bold"�6d      HintOpacity$	 .26Q�6w   $��20 0 0 0q�6�    �� 
IsEditable .  �� MaxDropDownHeight$	 150��5W   Z   ��6#   �� *MyDataSource+6r   5Z      5[      �� 5\      $��2��6   $��Center=�6*   $��5q�6E   .+�5]      �� $!��SN/SSN/CARTON_NO/MCARTON_NO��6   $��16(�6B   #��[ 6Q   6|   �� 5^      ���	#FF6F5CC7$��1q�65    �� CornerRadius$ 5��6I   ���#FFFF._�5_      �� -   5        $��tbInputData��5_   "   $��30��66   6^   5`      5a      � 5b      $��3��6    5c      �� 5d      6-   �� 5e      6-   �� 5f      6-   .+�5h      � $��0��6   .+�5i      �� -   5        $��btnOK��5i   !   
�������$	��100��6A   $��OK��6M   #��[ 6[   6�   5j      � 5k      $��1��6   .+�5l      �� -   5        $��	btnCancle��5l   !   $	��100��62   $��Cancle��6>   
�������#��[ 6b   6�   5m      � 5n      $��2��6   .+�5o      �� -   5        $��btnClose��5o   =   $	��100��6!   $��Close��6-   
�������#��[ 6`   6�   5p      5q      � 5r      $��4��6   .+�5s      ]� $��10 0 0 0q�6    .+�5t      d� -	   5        $��cbtabelZ��5t   C   $
��Left=�6(   #��[ 5u      $, &MaterialDesignSwitchAccentToggleButton��5v      $ FalseY�5w      $��2��6-   5x   (   �� 5z      $��Z_WIP_TRACKING��6    
�������$��10 0 0 0q�6M   {�6_   �� $ 	IsChecked�$
 cbtabelZ��5{      5}      5~      � 5      $��1��6   $��10q�6#   $y�Hidden=�6/   .+�5�      &   System.Windows.Controls.DataGrid  -
   5          Name$ 
dataGridsn��5�         HorizontalGridLinesBrush �	#FFE24343
���   ����	#FFD9E4E66�   5�      � 5�      $��1��6   ���	#FFD9E4E6$��10q�6<   .+�5�      �� -   5         ��Name$ 
listDataSN��5�      $��16(�6/   ��5�      �� 5�       �� 
TargetType" ��6     �� Triggers 5�      P� 5�   "    P� Property �@J�IsMouseOver P� Value . . �5�   &   ��   �� Property  � ��! �� Value! �	#FFEAAF756C   5�   #   .�5�      ��   � ��$! 16(�69   �� 5�        � ��! �	#FFA030106<   �� 5�        � ��$	! 0 1q�6@   �� 5�        � ��$! 2q�67   �� 5�        � ��$	! 300��66   �� 5�        � ��$! Center=�6B   �� 5�        � ��$
! auto��67   5�      5�      5�      5�      5�      5�      