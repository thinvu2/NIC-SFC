   M S B A M L   `   `   `  ����G  CPackingBoxID, Version=1.0.4.2, Culture=neutral, PublicKeyToken=null3clr-namespace:PACKINGBOXID_CFGPACKINGBOXID_CFG  5   	        PACKINGBOXID_CFG.FormBOXID   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:PACKINGBOXID_CFG     5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   Bi:http://schemas.microsoft.com/expression/2010/interactivity  5	   	     � WindowStartupLocation$  CenterScreen=�5
   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   	      TextRenderingMode$
 Auto=�5   	   $��Arial)�5   	     MaterialDesignPaper#���  5   	    � Title$ 	FormBOXID��5   	   $	��300��6   $	��400��6(   .��5      � ��5   
   �� 5       �� EndPoint$ 	0.504,1.5&�6"    �� 
StartPoint$ 
0.504,0.03&�5   "   .��5      �  � Color$ #034E70��6    � Offset$ 0Q�6/   � 5      $ #0889A6��6   $ 0.567Q�6/   5       � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��3* �6   �� 5      $��1* �6   .+�5!   
   � $��0��6   .+�5"      �� $��BOXID��6   
�������$��Center=�67   $��1,8,1,9q�6R   $��2��6c   5$      5%      � 5&   
   $��1��6   $��Center=�6   $
��13 0q�67   .+�5'      #  MaterialDesignThemes.Wpf.Card  .��5(      �� -   5        	 ��Name$	 	Edt_BOXID��5(      $��30��6-   69   5)      5*      � 5+   
   $��2��6   $
��10 0q�6   
 � ColumnDefinitions
 5,      �� 5-      �� 5.      .+�50      � $��0��6   .+�51      �� -   5         ��Name$ 	Item_Name��51      
�������$��3q�6@   52      � 53      $��1��6   .+�54      �� -   5        $ 	Item_Data��54      
�������$��3q�6@   55      56      � 57   
   $��3��6   
 58      �� 59      �� 5:      �� 5;      .+�5=      � $��0��6   $��Center=�6#   .+�5>      �� -   5         ��Name$ Btn_OK��5>      
���2͚�
���  ��$��90��6S   ��6s   Z� .��5?      ��  �� Orientation$ 
Horizontal=�6!   .+�5@      '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=null]]   Kind$ Check=�62   �� 5A      $��OK��6$   60   5B      5C      5D      � 5E      $��1��6   $��Center=�6#   .+�5F      �� -   5        $ 
Btn_Cancel��5F      
���2͚�
���  ��$��90��6W   ��6b   Z� .��5G      �� $ 
Horizontal=�6!   .+�5H        $ Close=�62   �� 5I      $��Cancel��6$   64   5J      5K      5L      � 5M      $��2��6   $��Center=�6#   .+�5N      �� -   5        $ 	Btn_Close��5N      
���2͚�
���  ��$��90��6V   ��6a   Z� .��5O      �� $ 
Horizontal=�6!   .+�5P        $ Close=�62   �� 5Q      $��Close��6$   63   5R      5S      5T      5U      5V      5W      