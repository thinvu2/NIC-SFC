   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	   #clr-namespace:PACK_CTNPACK_CTN  5   	       PACK_CTN.InputBox   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   (localclr-namespace:PACK_CTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	   -   5        $��	InputPass��5   	      MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��13(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignPaper#���  5   	     MaterialDesignFont#���  5   	    � WindowStyle$
 None=�5   	    � 
ResizeMode$ NoResize=�5   	    � Title$ Input Password��5   	   $	��200��6    $	��400��6-    � WindowState$ Normal=�69    � WindowStartupLocation$ CenterScreen=�6g   .��5      � ���#0889a6 � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      *   $PACK_CTN.UserControlCTN.ControlBarUC  
 � Tag 6   �� *Title+	 �� ElementName$	 	InputPass��6M   5      � 5    
   $��1��6   $��1q�6   ��5!      �� 5"      
 �� EndPoint$
 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5#      �  � Color$ 	#FF36C6E6��6#    � Offset$	 0.0Q�65   � 5$      $ 	#FFC6E0E6��6#   $	 0.0Q�65   5%      .+�5(      � $��1��6    � 	Resources 5)       �� 
TargetType �� Property �� Value��%^   '�       �� 5*      " �6   .�5+      ��  � ��$ 	15 0 15 0q�63   6G   5,      5-       5.      �� 5/      6&   �� 50      6&   �� 51      6&   .+�53      � $��0��6   .+�54      �� -   5        $��tbMess��54       $6��0Reprint Password , Please input IPQC's password ��6.   
���  ��$��Center=�6x   $��16(�6�   $��WrapWithOverflow=�6�   6�   55      � 56      $��1��6   .+�57      G� -   5        $��	inputPass��57   "   
�������
���   �$��1q�6Z   $��35��6o   $	��250��6{    G� PasswordChar$ *��6�   $��30(�6�   6�   58      � 59      $��2��6    � ColumnDefinitions 5:      �� 5;      6-   �� 5<      6-   .+�5>      �� -   5        $��	btnCancle��5>   -   $��0��6    # MaterialDesignRaisedLightButton#��[ 6?   $	��100��6z   
�������$��Cancle��5?   0   ���#0889a65A      �� 5B      -   5        $��btnLogin��5B   -   $��1��6   #��[ 6=   $	��100��6w   
�������$��Login��5C   0   ���#0889a65D      5E      5F      5G      5I      5J      