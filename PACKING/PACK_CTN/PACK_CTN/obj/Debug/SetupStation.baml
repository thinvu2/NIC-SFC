   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=null#clr-namespace:PACK_CTNPACK_CTN  5   	   A%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	       PACK_CTN.SetupStation   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:PACK_CTN     5   	   4uc%clr-namespace:PACK_CTN.UserControlCTN     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5
   	   -   5        $��staionsetup��5   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	   ���	#FFE5F2F5 � Title$ SetupStation��5   	   $	��300��6   $	��400��6+    � WindowState$ Normal=�67    � WindowStyle$
 None=�6L    � WindowStartupLocation$ CenterScreen=�5      .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      *   $PACK_CTN.UserControlCTN.ControlBarUC  
 � Tag 6   �� *Title+ �� ElementName$ staionsetup��6P   5      � 5   
   $��1��6   ���#0889a6 5      �� 5      6"   �� 5      $
��0.5* �6    60   �� 5       6"   .+�5"      � $��0��6   
�������$��	2 1 2 0.5q�63   	 � ColumnDefinitions	 5#      �� 5$      $��2* �6'   64   �� 5%      $��3* �6'   64   .+�5'      � $��0��6   $
��10 0q�6'   .+�5(      �� -   5        $��cbChangeLine��5(      ���	#DDD12424���	#FF29D3EE���	#FF52CAD66{   �� 5)      $��Center=�6   
���   �$��Center=�6L   $��Change Line��6g   6�   5*      � 5+      $��1��6   $
��10 0q�6'   .+�5,      �� -   5        $��cbbLineName��5,      $��Center=�62   
���   �
 �� 
IsEditable
 .  �� MaxDropDownHeight$	 150��6�   ��6`   �� *MyDataSource+6�   5-      5.      � 5/      $��1��6   
�������$��2 0.5q�63   	 50      �� 51      $	��71* �6'   �� 52      $
��124* �6'   .+�54      � $��2��6   .+�55      �� $��Center=�6   $��Center=�68   $��30��6U   $��Input pass :��6a   
���   �6�   G� 57      -   5        $��IputPassword��57   #   $��Center=�67   $��Right=�6R    G� PasswordChar$ #��6n   $	��100��6   $��0,0,26,7q�6�   
����������#0889a6
���   �6�   59      5:      � 5<      $��3��6   
�������$��	2 0.5 2 1q�63   	 5=      �� 5>      6)   �� 5?      6)   .+�5A      � $��0��6   .+�5B      �� -   5        $	��100��5B      $��OK��6)   ���#0889a6���#0889a6
�������6�   5C      � 5D      $��1��6   .+�5E      �� -   5        $	��100��5E      $��Cancle��6)   ���#0889a6
����������#0889a66�   5F      5G      5I      5K      5L      