   M S B A M L   `   `   `  ����H  DPACK_PALT_NEW, Version=1.0.4.8, Culture=neutral, PublicKeyToken=null%clr-namespace:PACK_PALT	PACK_PALT  5   	       PACK_PALT.LabelQty   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:PACK_PALT     5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	      MaterialDesignBody#���   5	   	   $��Regular"�5
   	   $��14(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignFont#���  5   	    � Title$ LabelQty��5   	   $��137.815��6   $��430.926��6+    � WindowStartupLocation$ CenterScreen=�6;   .��5      � ��5   
   �� 5       �� EndPoint$ 	0.504,1.5&�6"    �� 
StartPoint$ 
0.504,0.03&�67   .��5      �  � Color$ #03405D��6    � Offset$ 0Q�6/   � 5      $ #0AABCF��6   $ 0.567Q�6/   5       � RowDefinitions 5   
   �� 5      $��2* �6   �� 5      .+�5   
   � $��0��6   	 � ColumnDefinitions	 5      �� 5      $��1* �6#   60   �� 5      $��2* �6#   60   .+�5       � $��0��6   .+�5!      #  MaterialDesignThemes.Wpf.Chip  
������� ! PrimaryHueDarkForegroundBrush
   IconForeground#
 �  69     PrimaryHueDarkBrush   IconBackground# �  6{   .��6�   Pallet label Qty:5#      5$      � 5&      $��1��6   $��Center=�6#   	 5'      �� 5(      �� 5)      $��60 �6'   .+�5+      � $��0��6   $��	15 15 0 0q�6'   .+�5,      #  MaterialDesignThemes.Wpf.Card  .��5-      �� -   5         ��Name$ txtNum��5-   "   
�������;  5MaterialDesignThemes.Wpf.Transitions.TransitionAssist   DisableTransitions . $��40��6   $��22(�6�   {�.  5.   D   5/      50      � 52      $��1��6    53      �� 54      $	��30* �6(   �� 55      $	��30* �6(   .+�57      � .+�58      (� $��Stretch=�6"   $��Stretch=�6@   $
��Auto��6\   $
��Auto��6j   .��59      '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=null]]   Kind$ 	ChevronUp=�66   
�������$��32.941��6Z   $��20��6j   5:      �� 5;      -   5         ��Name$ cmdUp��5;   !   $
��Auto��6^   ��6l   Z� ��6�   Z� 5<      � 5=      $��1��6   .+�5>      (� $��Stretch=�6"   $��Stretch=�6@   $
��Auto��6\   $
��Auto��6j   .��5?        $ ChevronDown=�66   
�������$��32.942��6\   $��20��6l   5@      �� 5A      -   5        $ cmdDown��5A   !   $
��Auto��6b   ��6p   Z� ��6�   Z� 5B      5C      5D      5E      � 5F   
   $��1��6   .+�5G      �� -   5        $��bbtnOK��5G   �    $  MaterialDesignRaisedAccentButton#��[ 6   $	��100��6O    � ToolTip$5 /Resource name: MaterialDesignRaisedAccentButton��6[   {�. $��OK��6�   ���	#FF7349C15H      5I      5J      5K      