   M S B A M L   `   `   `  ����G  CCodeSoft_New, Version=1.0.1.4, Culture=neutral, PublicKeyToken=null;"clr-namespace:CodeSoft_9Codes.ViewCodeSoft_9Codes.View  5      '    !CodeSoft_9Codes.View.ReleaseLabel   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:CodeSoft_9Codes.View     5      -   5        $��Courier New)�5       $��16(�69   .��5	      �� $��3q�6   ���#FFF  �� CornerRadius$  5��64   ._�5
   
   � ��5      �� 5       �� EndPoint$ 	0.504,1.5&�6&    �� 
StartPoint$ 
0.504,0.03&�6;   .��5      �  � Color$ #034E70��6#    � Offset$ 0Q�63   � 5      $ #0889A6��6#   $ 0.567Q�63   5       � RowDefinitions 5      �� 5      $��25 �6    �� 5      $	��150 �6    �� 5      $��* �6    �� 5      $��30 �6    .+�5      o� $	��Top=�6   .@�5      m� $��_Config��6   .@�5      m� -   5         m�Name$ 
miSetmodel��5       m� IsCheckable . $��_Set Model Data��6F   6_   m� 5      -   5        $ 
miSetPrint��5       . $��_Set Print Data��6F   6_   m� 5      -   5        $ miQuery��5      $��_Query��60    . 6S   m� 5      $��_Auto��6   �� 5      m� 5      $��_Exit��6   5      5       �� 5!      $��1��6   $��5 5 5 5q�6#   ���#fff$	��1.5q�6G   $  5��6]    �� Padding$ 0 0 5 0q�6n   ._�5"      � 	 � ColumnDefinitions	 5#      �� 5$      $	��150 �6+   �� 5%      $	��250 �6+   �� 5&      $	��250 �6+   �� 5'      $	��200 �6+   �� 5(      $	��200 �6+   �� 5)      .+�5+      � $��0��6    5,      �� 5-      6.   �� 5.      6.   �� 5/      6.   .+�51      � $��0��6   .+�52      �� $
��Bold"�6$   $��16(�66   $��0 0 10 0q�6D   ���#FFF$��Center=�6h   $��Right=�6�   .��6�   MO :6�   53      � 54      $��1��6   .+�55      �� $
��Bold"�6$   $��16(�66   $��0 0 10 0q�6D   ���#FFF$��Center=�6h   $��Right=�6�   .��6�   	MODEL :6�   56      � 57      $��2��6   .+�58      �� $
��Bold"�6$   $��16(�66   $��0 0 10 0q�6D   ���#FFF$��Center=�6h   $��Right=�6�   .��6�   QTY :6�   59      5:      � 5;      $��1��6   
 � 	Resources
 5<       �� 
TargetType �� Property �� Value   TextBoxBaseControlTemplate �� 
TargetType ��Name ��Name �� Triggers P� Property P� Value �� 
TargetName��%�  '��       &	  �     �� 5=      " ��6$   .�5>   "   ��  � ��$
 Bold"�6?   6N   �� 5?   "    � ��$ Center=�6M   6^   �� 5@   "    � ��$ Center=�6O   6`   �� 5A   "    � ��$ Center=�6F   6W   �� 5B   "    � ��$ 5q�6<   6H   5C      �� 5D      " ��6Q   .R�5E   "   �� $ Bd��5F   1   #��z��5E   )   
���   �#��z��5G   1   $  10��6e   ._�5H   &   �� $ PART_ContentHost��63   5I   #    5J   "   P� 5K   &    � {� .  . �5L   *   �� # � ��6G   $ Bd��6�    � ���� 5M   *   # � ��6G    � ��5N   '   P� 5O   &    � ��$
 Auto��6?   . �5P   *   ��  � ��$	 100��6E   5Q   '   P� 5R   &    � ��$
 Auto��6@   . �5S   *   ��  � ��$ 20��6F   5T   '   5V      5W       5X      �� 5Y      6.   �� 5Z      6.   �� 5[      6.   .+�5]      � $��0��6   .+�5^      �� -   5         ��Name$ txtMo��5^   &    � Cursor$
 Hand��65   #��[  6C   $��0,3,0,2q�6�   6�   5_      � 5`      $��1��6   .+�5a      �� -   5        $ txtModel��5a   &   #��[  68   ���#FFF1D1 �� 
IsReadOnly . 6�   5b      � 5c      $��2��6   .+�5d      �� -   5        $ txtTargetQty��5d   &   #��[  6<   ���#FFF1D1 . 6�   5e      5f      �� 5h      $��2��6   $��5 5 5 5q�6-   ���#fff$	��1.5q�6Q   $  5��6g   ._�5i      �� -   5         ��Name$ lvListMORange��5i   #   6>   5j      � 5k      $��3��6   $��5 5 5 5q�6+    5l      �� 5m      6.   �� 5n      6.   .+�5p      �� 
�������$��2q�65   $��40��6I   $  3��6U   $��Right=�6f   $��0,0,0,0q�6�   $	��150��6�   ._�5q      �� -	   5         ��Name$ btnInput��5q   %   $
��Bold"�67   $��Input >>��6I   $��26(�6\   
�������
�������$��3q�6�   $
 Hand��6�   {�.  ��5r   "   �� 5s   &   " ��6,   .�5t   *   ��  � �� �#034E706G   �� 5u   *    � �� 5v   .   �� 5w   2   " ��6B   .R�5x   6   �� $ Border��6=   #��z��6M   ._�5y   :   �� $��Center=�6K   $��Center=�6h   5z   7    5{   6   P� 5|   :    �@J�IsMouseOver . . �5}   >   �� $ Border��6k    � �� �#0889A65~   ;   5�   3   5�   +   5�   '   5�      5�      �� 5�      $��1��6!   
�������$��2q�6B   $��40��6V   $  3��6b   $��Right=�6s   $��0,0,0,0q�6�   $	��150��6�   ._�5�      �� -
   5        $ btnClear��5�   %   $
��Bold"�67   $��Clear >>��6I   $��26(�6\   
�������
�������$��3q�6�   $
 Hand��6�   {�.  ��5�   "   �� 5�   &   " ��6,   .�5�   *   ��  � �� �#034E706G   �� 5�   *    � �� 5�   .   �� 5�   2   " ��6B   .R�5�   6   �� $ Border��6=   #��z��6M   ._�5�   :   �� $��Center=�6K   $��Center=�6h   5�   7    5�   6   P� 5�   :    �@J�IsMouseOver . . �5�   >   �� $ Border��6k    � �� �#0889A65�   ;   5�   3   5�   +   5�   '   5�      5�      5�      � 5�      $��4��6   
 5�      ��%�  '��       &	  �     �� 5�      " ��6$   .�5�   "   ��  � ��$
 Bold"�6?   6N   �� 5�   "    � ��$ Center=�6M   6^   �� 5�   "    � ��$ Center=�6O   6`   �� 5�   "    � ��$ Center=�6F   6W   �� 5�   "    � ��$ 5q�6<   6H   5�      �� 5�      " ��6Q   .R�5�   "   �� $ Bd��5�   1   #��z��5�   )   
���   �#��z��5�   1   $  10��6e   ._�5�   &   �� $ PART_ContentHost��63   5�   #    5�   "   P� 5�   &    � {� .  . �5�   *   �� # � ��6G   $ Bd��6�    � ���� 5�   *   # � ��6G    � ��5�   '   P� 5�   &    � ��$
 Auto��6?   . �5�   *   ��  � ��$	 100��6E   5�   '   P� 5�   &    � ��$
 Auto��6@   . �5�   *   ��  � ��$ 20��6F   5�   '   5�      5�      .+�5�      �� -   5         ��Name$ 
gbOverFlow��5�   #   $y�Hidden=�67   $��
AUTO CHECK��6K   ���#034E70
�������$
��Bold"�6�   $��5q�6�   .��5�      �  � Orientation$ 
Horizontal=�6(   $��Center=�6A   ���#034E70.+�5�   "   �� $
��Bold"�6(   $��16(�6:   $��0 0 0 0q�6H   ���#FFF$��Center=�6k   $��Center=�6�   $
��Left=�6�   .��6�   QTY :6�   �� 5�   "   -   5        $ txtOverFlow��5�   *   #��[  6?   $	��110��6v   $
 Hand��6�   $��Center=�6�   $��Center=�6�   $
��Left=�6�   $��0,3,0,2q�6  6$  5�      5�      5�      � 5�      $��5��6   $y�	Collapsed=�6+    5�      �� 5�      �� 5�      .+�5�      � $��0��6$   $ 
Horizontal=�61   $��Center=�6J   ���#034E70.+�5�      �� $
��Bold"�6$   $��16(�66   $��0 0 0 0q�6D   ���#FFF$��Center=�6g   $��Center=�6�   $
��Left=�6�   .��6�   
SUCCESS:6�   �� 5�      -   5         ��Name$ 
lblSuccess��5�   $   $
��Bold"�68   $��16(�6J   $��0 0 0 0q�6X   ���#FFF$��Center=�6{   $��Center=�6�   $
��Left=�6�   .��6�   06�   5�      � 5�      $��1��6$   $ 
Horizontal=�61   $��Center=�6J   ���#034E70.+�5�      �� $
��Bold"�6$   $��16(�66   $��0 0 0 0q�6D   ���#FFF$��Center=�6g   $��Center=�6�   $
��Left=�6�   .��6�   FAIL :6�   �� 5�      $
��Bold"�6%   $��16(�67   $��0 0 0 0q�6E   ���#FFF$��Center=�6h   $��Center=�6�   $
��Left=�6�   .��6�   06�   5�      5�      5�      5�      �� 5�      -   5        $ bdInput��5�      $��2��6&   $��5 5 5 5q�63   ���#fff$	��1.5q�6W   $  5��6m   ._�5�      � -   5         �Name$ BG��5�      $��0,0,0,0q�6$   ��5�      �� 5�      -   5        
img_bg�� �� ImageSource$ ../Assets/fii_bg.png��5�   !    j� Stretch$
 Fill=�5�   !   5�      5�      5�      5�      5�      