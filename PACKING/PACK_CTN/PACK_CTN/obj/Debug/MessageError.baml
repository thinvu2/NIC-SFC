   M S B A M L   `   `   `  ����C  ?PACK_CTN, Version=1.1.0.5, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PACK_CTN.UserControlCTNPACK_CTN.UserControlCTN  5   	   #clr-namespace:PACK_CTNPACK_CTN  5   	       PACK_CTN.MessageError   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   .uc%clr-namespace:PACK_CTN.UserControlCTN  5   	   "localclr-namespace:PACK_CTN  5   	   -   5        $��
MessageFrm��5   	     � WindowState$  Normal=�5
   	    � WindowStyle$
 None=�6    � Title$ 
FrmMessage��5   	   $	��350��6   $	��550��6)   $��Arial)�65   $��14(�6H    � WindowStartupLocation$ CenterScreen=�6s   .��5      � ���#0889a6 � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      *   $PACK_CTN.UserControlCTN.ControlBarUC  
 � Tag 6   �� *Title+ �� ElementName$ 
MessageFrm��6P   5      � 5   
   $��1��6   ��5      �� 5       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5      � 	 � Color$	 	#FF36C6E6��6#   
 � Offset$
 0.975Q�65   � 5      $	 	#FFC6E0E6��6#   $
 0.519Q�65   5       5      �� 5      6"   �� 5      6"   �� 5      $
��0.8* �6    61   .+�5!      � $��0��6   .+�5"      ��  �� CornerRadius$ 10��6   ���	#FFCF2323$��1q�6C   $��5q�6W   
�������._�5#      �� -   5        $��
lblEnglish��5#       $��16(�62   
���  ��$
��Left=�6Q   $	��Top=�6l   $��WrapWithOverflow=�6�   $��5q�6�   5$      5%      � 5&      $��1��6   .+�5'      �� $ 10��6   ���	#FFCF2323$��1q�6C   $��5q�6W   
�������._�5(      �� -   5        $��lblVietNamese��5(       $��16(�65   
���  ��$
��Left=�6T   $	��Top=�6o   $��WrapWithOverflow=�6�   $��5q�6�   5)      5*      � 5+      $��2��6    � ColumnDefinitions 5,      �� 5-      6)   �� 5.      6)   .+�50      G� -   5        $��txtPassword��50      $��0��62   $	��120��6B   $��50��6N   $��Center=�6[   $��Right=�6v   $��0 0 20 0q�6�   $��30(�6�   
����������	#FF7349C1$��1q�6�    G� PasswordChar$ *��6  �� 52      -   5        $��btnOK��52      $��1��6'   $	��150��67   $��Center=�6C   $
��Left=�6^   $��20 0 0 0q�6z   $��50��6�    #  MaterialDesignRaisedLightButton#��[  6�    � ToolTip$4 .Resource name: MaterialDesignRaisedLightButton��6�   $��OK��6  $��20(�6  ���	#FF7349C1$��1q�6>  
����������#f2561d53   X   54      56      58      59      