   M S B A M L   `   `   `  ����F  BMake_Weight, Version=1.0.5.2, Culture=neutral, PublicKeyToken=null)clr-namespace:Make_WeightMake_Weight  5   	       Make_Weight.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Make_Weight     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	    � AllowsTransparency .  � 
ResizeMode$ 	CanResize=�6H    � WindowStyle$
 None=�6_    � Icon$ Images/scale5.ico��5
   	    � Title$ MAKE WEIGHT��5   	   $	��750��6   $
��1200��6*    � 	Resources 5         GotFocusColor �� Color  LostFocusColor �� 
TargetType  btnStyle	 �� Property
 �� Value �� 
TargetType �� CornerRadius ��Name �� Triggers P� Property P� Value)  #System.Windows.Controls.DataGridRow  ComboboxButtonStyle ��Name  ComboboxTextBoxStyle ��Name  ComboBoxItemStyle  ComboboxStyle � ColumnDefinitions �� 	ClickMode d� 	IsChecked �� Path �� Mode �� RelativeSource �� SelectionBoxItem �� SelectionBoxItemTemplate �� IsDropDownOpen � AllowsTransparency J� SnapsToDevicePixels � ActualWidth �� MaxDropDownHeight  K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation!   DirectionalNavigation=  7System.Windows.Controls.Primitives.DataGridColumnHeader  TextBoxBaseControlTemplate*  $System.Windows.Controls.DataGridCell   TextBoxWeightControlTemplate 	 PWBoxBaseControlTemplate��%�  &	        &	 #     &	 H     '  n    &	 s    &	 �    &	 N    &	 �    7  7  7  '  m    &	 (    '  �    &	 Z    &		 n    �� 5   
   $ 	#FFECECD6��60   �� 5   
   $ Transparent��61   �� 5   
   " ��6   .�5      �� 	 �@�OverridesDefaultStyle
 . 66   �� 5      	 � ��

 �����6+   �� 5      	 � ��

 �   �6+   �� 5      	 � ��
 5      �� 5      " ��6&   .R�5      �� $ 10��6!   
�������$��0,0,0,0q�6G   ._�5      �� $ contentPresenter��6/   #��z��6I   #��z��6}   #��z��6�   #��z��6�   #��z��6  5      5      5       5      P� 5       � {� .  . �5       �� 	 � ��

 �����63   �� 5!      	 � ��

 �����63   5"      5$      �� 5%   
   "  6   .�5&      �� 	 � ��
 �	#FF0000006+   �� 5'      	 � ��$
 25��6'   �� 5(      	 � ��$
 Stretch=�6;   �� 5)      	 � ��$
 11(�6)   �� 5*      	 � ��$
 Arial)�6+    5+      P� 5,       �@ 
IsSelected . . �5-      �� 	 � ��
 �#FFCC3363   �� 5.      	 � ��
 �	#FF00000063   5/      P� 51       �@ 
IsSelected .  . �52      �� 	 � ��

 �����63   �� 53      	 � ��
 �	#FF00000063   54      57      �� 59   
   " d�6,   .�5:      �� 	 � ��
 5;      �� 5<      " d�6&   .R�5=      �� $ border��5?      
�������$ 	0,10,10,0��5@      $��0,1,1,1q�5A      ���#FFCC33._�5C      �� 5D      5E      5G      �� 5H      	 � ��$
 Center=�6;   5I      �� 5K   
   " ��6-   .�5L      �� 	 � ��
 5M      �� 5N      " ��6&   .R�5O      �� $ 	10,0,0,10��6!   $��1,1,0,1q�5P      $��Stretch=�5Q      
�������{�. ���#FFCC33._�5U      �� $ PART_ContentHost��6+   5V      5W      5Y      �� 5Z      	 � ��$
 Arial)�6+   �� 5[      	 � ��$
 12"�6+   �� 5\      	 � ��$
 Center=�6;   �� 5]      	 � ��$
 Center=�69   5^      �� 5`   
   " ��6*    5a      P� 5b       �@��
IsSelected . . �5d      �� 	 � ��$
 Center=�6C   5e      5g      �� 5i   
   " ��6&   .�5j      �� 	 � ��$
 Center=�6;   �� 5k      	 � ��$
 Center=�69   �� 5l      	 � ��
 5m      �� 5n      " ��6&   .R�5o      �  5p      �� 5q   "   $	��185 �63   �� 5r   "   $��15 �63   .+�5t      �� 8��  6X   �� 5u      $��PART_EditableTextBox��6&   8�� $��Center=�5w      {�. $��0,0,0,0q�5y      $��22��5z      d� 5{      $��1��6+   $��0q�6;   $��22��5|      8�� |�.  $ Press=�5�       5      �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+.��5�   "   F� $��1��6'   $��Center=�5�      $��Center=�5�      "����        � 	=  	= � z     	
������5�      5�      �� 5�      $��ContentSite��6/   #��z 5�      #��z 5�      #��z��5�      $��Center=�5�      $��Center=�5�      $��5,0,0,0q�5�      � 5�      $��Popup��6$   $��Bottom=�5�      #��z 5�       .  |�.  $��Slide=�5�      .��5�   "   � $��DropDown��6'    . #��z 5�      #��z 5�       5�   &   �� 5�   *   $
��163* �6;   �� 5�   *   $	��11* �6;   .+�5�   &   �� $ DropDownBorder��5�      $��1q�5�      $ 10��5�      ���	#FF23536A���#FFCC33$��2��5�   *   �� 5�   &   $��4,6,4,6q�63    . $��2��6_   .��5�   *   ��   . $! 	Contained=�6H   ���	#FF23536A6}   5�   '   5�   #   5�      5�      5�      5�      5�      �� 5�   
   "  6   .�5�      �� 	 � ��
 �#FFCC336+   �� 5�      	 � ��
 �	#FF0000006+   �� 5�      	 � ��$
 Center=�6;   �� 5�      	 � ��$
 Center=�69   �� 5�      	 � ��
 .  6*   �� 5�      	 �@ SeparatorBrush
 �	#FFC9CACA6/   �� 5�      	 � ��$
 4q�6(   �� 5�      	 � ��$
 Arial)�6+   �� 5�      	 � ��$
 18(�6)   �� 5�      	 � ��$

 Bold"�6+   5�      �� 5�   
   " ��63   .�5�      �� 	 � ��
 5�      �� 5�      " ��6&   .R�5�      �� $ Bd��5�      
����������#FFCC33{�. #��z��5�      $ 10��6E   ._�5�      �� $ PART_ContentHost��5�      $��0q�5�   +   5�      5�      5�      5�      �� 5�      	 � ��$
 Arial)�6+   �� 5�      	 � ��$
 13"�6+   �� 5�      	 � ��$
 Center=�6;   �� 5�      	 � ��$
 Center=�69   5�      �� 5�   
   "  6   .�5�      �� 	 � ��$
 Center=�64   �� 5�      	 � ��$
 18(�6)   5�      �� 5�   
   " ��65   .�5�      �� 	 � ��
 5�      �� 5�      " ��6&   .R�5�      �� $ Bd��5�      
����������#FFCC33{�. #��z��5�      $ 10��6E   ._�5�      �� $ PART_ContentHost��5�      $��0q�5�   +   5�      5�      5�      5�      �� 5�      	 � ��$
 Arial)�6+   �� 5�      	 � ��$

 Bold"�6+   �� 5�      	 � ��$
 16(�6)   �� 5�      	 � ��
 �	#FF32C7326+   �� 5�      	 � ��$
 Center=�6;   �� 5�      	 � ��$
 Center=�69   5�      �� 5�   
   " G�61   .�5�      �� 	 � ��
 5�      �� 5�      " G�6&   .R�5�      �� $ Bd��5�      
����������#FFCC33{�. #��z��5�      $ 10��6E   ._�5�      �� $ PART_ContentHost��5�      $��0q�5�   +   5�      5�      5�      5�      �� 5�      	 � ��$
 Arial)�6+   �� 5      	 � ��$
 13"�6+   �� 5     	 � ��$
 Center=�6;   �� 5     	 � ��$
 Center=�69   5     5     .��5     �� -   5        $ borderFrame��5     $��0,0,2,0q�6"   
�������$��1,1,1,23q�6G   ���	#FF23536A._�5  
   � " � RowDefinitions" 5     �� 5	     $��4* �6    �� 5
     $
��117* �6    �� 5     $��0* �6    .+�5     � -   5        # �Name$# containerFrame��5     $��	0,0,0,-23q�6+   $��3��6>   .+�5     � -   5        $# 
homeHeader��5     $	��Top=�6+   $��Stretch=�6C   $��81��6a   ���	#FF23536A.+�5     �� $��38.099��6   $ J� Clip4$ ��        ����'    ����'���'�    ���'�        $
��Left=�6h   $��8,8,0,13.833q�6�   .+�5     F� $��37.334��6   $��37.337��6.   % �� Stretch$
% Fill=�6>   �����(�y0� � �a!��Y(WA�w�,w���x�)(��!��I	�b�#'Ө,%���0�y%���`Fg$`���W�#5���!Q?X9�Ȫ�@؟X �pI��lw"�����#@��ؔ�$Ө�����%0�j�)% #���#�0Z
�("0r��� ��!o���l�@�\=*P���\��4��sӜ��HPL�� ��-��P@�h�Ӥ�X?�V����po����Ӡv��w����p�L(�y0� ��5     �� 5  "   $
 Pink��62   5     F� 5     $��29.175��6   $��31.526��6.   & �� Left$& 4.078��6>   
' �� Top$
' 2.86��6R   $
% Fill=�6d   
������������@"�d��x�H�I ߹`���kn�
���u 4<��z�!0۸H�H	�P�� 5�	 ��RY
����
���Mb�@ ��&1���@@1, �#  a��0K;!�"�0��! �@�!ȇS��#� Pq��|�2"���@���.�@��H5�u��R�pw����u���;pw�P;�� 	��!p���r"�3�Йi"� �_Ȏr"0\�� !p$*
Ю�xd-
�-8�,
P ���+
�5)�0j.
������	H�0�	"�� �Hy�p�� ��,��2`�0�@���������
P��
Ӱ2
H��
x�*
�ز
8�,
Pݐ
Ӏ�+
�i#
�+,
)�	p�,
��H	��q z��8`�af@"�d�6�   F� 5     $��15.107��6   $��13.373��6.   $& 11.157��6>   $' 12.416��6S   $
% Fill=�6g   �����0�tP�MӸy� N@* N���P�M����`La���@|p�����ӸFi�`u��� �U�n��4Ӏ�5�V4��`X5����V4Ӡ�%��N�܎��m �����Ӏ��X����A�0�t�t���&1�
p@PJ� �0�ԓp�����8�0@| L��a0�tP�M��5     �� 5  "   $
 Blue��62   5     F� 5     $��6.364��6   $��5.639��6-   $& 19.396��6<   $' 22.751��6Q   $
% Fill=�6e   �����0�i��l
 ����	��8~X���hX0�0�� Â��n�3eѸ�p�tӰ��XG訇8x	���Ӑ�����5 ����Y��p.� -���A����3r�8� R������0�i���5     �� 5  "   $
 blue��62   5     5      �� 5!     -   5        ( ��Name$( lblTitle��5!      $�� ��62   
�������$	��Top=�6M   $��Stretch=�6e   $��15(�6�   $��54,16,906,0q�6�   $��CharacterEllipsis=�6�   $��Arial)�6�   ��6�   Z� �� 5"     $��
0,80,0,-21q�6    $��Stretch=�64   
�������$	��0.8��6`   ]� 5$     .+�5%     o� $	��Top=�6   $��10,55,1132,0q�64   $��22��6J   ���	#FF23536A.@�5&     m� $��_Setup��6'   $��Arial)�67   $
��Bold"�6J   $��14(�6\   ���	#FFF0DB0C���	#FF23536A$��22��6�   .@�5'  "   m� -   5        $��	setupline��5'  ?   $��
Setup Line��6+   $��Arial)�6Q   $��Normal"�6d   
���   �) m� 	IsChecked) .  $��13(�6�   
�������6�   m� 5(  "   -   5        $��	CTN_CHECK��5(  A   $��Check Carton��6+   $��Arial)�6S   $��Normal"�6f   * m� IsCheckable* . 
���   �) .  $��13(�6�   
�������6�   m� 5)  "   -   5        $��
FQA_WEIGHT��5)  ?   $��
FQA Weight��6+   $��Arial)�6Q   $��Normal"�6d   
���   �* . ) .  $��13(�6�   
�������6�   5*     5+     �� 5,     6'   5-     ]� 5.     .+�5/     o� $	��Top=�6   $��64,55,1050,0q�64   $��22��6J   ���	#FF23536A.@�50     m� $��	_Language��6'   $��Arial)�6:   $
��Bold"�6M   $��14(�6_   ���	#FFF0DB0C���	#FF23536A$��22��6�   .@�51  "   m� -	   5        $��EN��51  <   $��English��6+   $��Arial)�6G   $��Normal"�6Z   * . ) . 
���   �$��13(�6�   6�   m� 52  "   -
   5        $��VI��52  ?   $��Tiếng Việt��6+   $��Arial)�6J   $��Normal"�6]   * . 
���   �) .  6�   53     54     �� 55     6'   56     ]� 57     .+�58     o� $	��Top=�6   $��150,55,990,0q�64   $��22��6J   ���	#FF23536A.@�59     m� -   5        $��_Label��59  '   $��Arial)�67   $
��Bold"�6J   $��14(�6\   ���	#FFF0DB0C���	#FF23536A$��22��6�   .@�5:  "   m� -   5        $��reprint��5:  <   $��Reprint��6+   $��Arial)�6K   * . $��Normal"�6r   ) .  
���   �$��13(�6�   
�������6�   m� 5;  "   -   5        $��visible��5;  <   $��Visible��6+   $��Arial)�6K   * . $��Normal"�6r   ) .  
���   �$��13(�6�   
�������6�   m� 5<  "   -   5        $��
Showparams��5<  @   $��Show Params��6+   $��Arial)�6R   $��Normal"�6e   * . ) .  
���   �$��13(�6�   
�������6�   5=     5>     �� 5?     6'   5@     �� 5A     -   5        $( lblTitleFunc��5A      $��Center=�66   $��Center=�6S   + �� TextAlignment$+ Center=�6n   $��
CTN_WEIGHT��6�   ���	#FFF0DB0C$��48(�6�   $��476,16,345,10q�6�   $��CharacterEllipsis=�6�   $��Arial)�6�   $��55��6#  $
��Bold"�6/  $	��375��6A  ��6  Z� 6A  �� 5B     -   5        , ��Name$, btnExit��5B     $��Right=�6I   $��
1156,0,0,0q�6e   $	��Top=�6y   $��35��6�   $��35��6�   ���	#FF23536A.��5C     �� .+�5D     �� $��/Images/exit.png��6$   5E     5F     �� 5G     -   5        $( lLine��5G      $��Center=�6/   $��Center=�6L   $+ Right=�6g   $
��LINE��6}   ���	#FFF0DB0C$��48(�6�   $��235,18,707,8q�6�   $��CharacterEllipsis=�6�   $��Arial)�6�   $��55��6  $
��Bold"�6   $	��254��62  ��6�   Z� 62  5H     � 5J     -   5        $# homeContent��5J     $��0,97,0,8q�6,   ���	#FF23536A 5K     �� 5L     $	��23* �6+   �� 5M     $	��22* �6+   �� 5N     $
��190* �6+   �� 5O     $
��961* �6+   .+�5R     �� -   5        - ��Name$- lblMacPrint��5R     $��
MO NUMBER:��61   $��Arial)�6F   
�������$
��Left=�6l   $��
152,63,0,0q�6�   $	��Top=�6�   $��14(�6�   . J� RenderTransformOrigin$. 0.553,3.304&�6�   $
��Bold"�6�   $��2��6�   $��2��6  �� 5S     -   5        $- 
lblVerCode��5S     $��STANDARD WEIGHT:��60   $��Arial)�6K   
�������$
��Left=�6q   $��150,158,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $	��155��6�   $
��Bold"�6�   $��2��6	  $��2��6  �� 5T     -   5        / ��Name$/ txtMoNumber��5T     #��[ 65   $
��Left=�6i   $��22��6�   $��
115,61,0,0q�6�   0 �� TextWrapping$
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��200��6�   $��Arial)�6�   $��3��6�   $��16(�6  $
��Bold"�6  {�.  6'  �� 5U     -   5        $/ txtStandardWeight��5U     #��[ 69   $
��Left=�6m   $��24��6�   $��115,154,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��200��6�   $. 0.506,2.957&�6�   $��Arial)�6  $��3��6   $��16(�60  {�.  $
��Bold"�6P  �� 5V     -   5        $- 	lblSeries��5V     $��MODEL NAME:��6/   $��Arial)�6E   
�������$
��Left=�6k   $��
374,61,0,0q�6�   $	��Top=�6�   $��14(�6�   $
��Bold"�6�   $��3��6�   �� 5W     -   5        $- lblLimit��5W     $��LIMIT RANGE:��6.   $��Arial)�6E   
�������$
��Left=�6k   $��376,154,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��3��6�   �� 5X     -   5        $- 	lblWebkey��5X     $��
UP WEIGHT:��6/   $��Arial)�6D   
�������$
��Left=�6j   $��374,113,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��3��6�   �� 5Y     -   5        $- lblSSID��5Y     $��DOWN WEIGHT:��6-   $��Arial)�6D   
�������$
��Left=�6j   $��152,109,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��2��6�   $��2��6
  �� 5Z     -   5        $/ txtDownWeight��5Z     #��[ 65   $
��Left=�6i   $��24��6�   $��115,109,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��200��6�   $��Arial)�6�   $��3��6�   $��16(�6  {�.  $
��Bold"�6(  �� 5[     -   5        $- label1��5[     $��	Password:��6,   $
��Left=�6@   $��373,0,0,602q�6[   $��Bottom=�6p   $��Arial)�6�   $��16(�6�   ���	#FFF9F4F4$
��Bold"�6�   $��3��6�   G� 5\     -   5        1 G�Name$1 	txtEmpPwd��5\  "   #��[	 65   $
��Left=�6g   $��23��6�   $��
509,17,0,0q�6�   $	��Top=�6�   $	��199��6�   $��Arial)�6�   {�. $��3��6�   $��16(�6�   $
��Bold"�6&  �� 5]     -   5        $/ txtModelName��5]     #��[ 64   $
��Left=�6h   $��22��6�   $��
509,65,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��201��6�   $��Arial)�6�   $��3��6�   $��16(�6  {�.  $
��Bold"�6&  �� 5^     -   5        $/ txtLimit��5^     #��[ 60   $
��Left=�6d   $��22��6   $��511,156,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��201��6�   $��Arial)�6�   $��3��6�   $��16(�6  {�.  $
��Bold"�6#  �� 5_     -    5        $/ txtUpWeight��5_     #��[ 63   $
��Left=�6g   $��22��6�   $��509,113,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��201��6�   $. 0.506,2.957&�6�   $��Arial)�6  $��3��6  $��16(�6*  {�.  $
��Bold"�6J  �� 5`     $��280,344,681,299q�6    $��Stretch=�69   
�������$	��0.8��6e   $��3��6r   &   System.Windows.Controls.DataGrid  5a     -!   5        2  Name$2 dtgRS��5a     3   
IsReadOnly3 . 4   SelectionMode$4 Single=�6A   5   AutoGenerateColumns5 .  $
��Left=�6t   $	��150��6�   $��
10,364,0,0q�6�   $	��Top=�6�   $
��1176��6�   
�������6   GridLinesVisibility$
6 None=�6�   $��Arial)�6  $��18(�6  $��4��6%  7   Columns7 5b     0  *System.Windows.Controls.DataGridTextColumn  5c     ,  &System.Windows.Controls.DataGridColumn8   Width5  /System.Windows.Controls.DataGridLengthConverter$	8 280 61   9   Header$9 	CARTON NO��6=   1	  +System.Windows.Controls.DataGridBoundColumn: 	  Binding: 6P   �� *	CARTON_NO+6q     5d     $	8 150 61   $9 WEIGHT��6=   : 6M   �� *CARTON_WEIGHT+6q     5e     $	8 150 61   $9 RESULT��6=   : 6M   �� *RESULT+6j     5f     $	8 300 61   $
9 NOTE��6=   : 6K   �� *NOTE+6f   5j     �� 5k     -"   5        $- lblMacPrint_Copy��5k     $��
CARTON NO:��66   $��Arial)�6K   
�������$
��Left=�6q   $��
48,211,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��2��6�   $	��118��6  �� 5l     -#   5        $- lblSN��5l     $��	SN : 0/0 ��6b   $��Arial)�6v   
�������$
��Left=�6�   $��
45,306,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $	��119��6  $
��Bold"�6!  $��2��63  �� 5m     -$   5        $/ txtCartonNo��5m     #��[ 6q   $
��Left=�6�   $��26��6�   $��189,211,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��315��6  $��Arial)�6!  $��2��64  $��16(�6D  {�.  $
��Bold"�6d  $��2��6�  �� 5n     -%   5        $/ txtSN��5n     #��[ 6-   $
��Left=�6a   $��26��6|   $��189,306,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��315��6�   $. 0.506,2.957&�6�   $��Arial)�6  $��2��6  $��16(�6$  $
��Bold"�62  $��2��6\  �� 5o     -&   5        $- lblSSID_Copy��5o     $��CARTON WEIGHT:��62   $��Arial)�6K   
�������$
��Left=�6q   $��
46,259,0,0q�6�   $	��Top=�6�   $��14(�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��2��6�   $	��134��6  �� 5p     -'   5        $/ txtCartonWeight��5p     #��[ 67   $
��Left=�6k   $��30��6�   $��	1,255,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��170��6�   $��Arial)�6�   $��3��6�   $��16(�6  {�.  $
��Bold"�6(  �� 5q     -(   5        $/ txtCOMP��5q     #��[ 6/   $
��Left=�6e   $��30��6�   $��176,255,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��140��6�   $��Arial)�6�   $��3��6�   ���	#FF818581$��16(�6  {�.  $
��Bold"�6;  �� 5r     -)   5        $- lblMacPrint_Copy1��5r     $��MCARTON:��67   $��Arial)�6J   
�������$
��Left=�6p   $��377,217,0,0q�6�   $	��Top=�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��3��6�   $��14(�6�   �� 5s     -*   5        $- lblVerCode_Copy1��5s     $��PRINT LABEL:��66   $��Arial)�6M   
�������$
��Left=�6s   $��376,256,0,0q�6�   $	��Top=�6�   $. 0.553,3.304&�6�   $	��140��6�   $
��Bold"�6�   $��3��6�   $��14(�6  �� 5t     -+   5        $/ txtPrintLabel��5t     #��[ 65   $
��Left=�6i   $��26��6�   $��509,256,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��161��6�   $. 0.506,2.957&�6�   $��Arial)�6	  $��3��6  $��16(�6,  {�.  $
��Bold"�6L  6^  �� 5u     -,   5        $- lblLineWeight��5u     $��LINE WEIGHT:��63   $��Arial)�6J   
�������$
��Left=�6p   $��377,301,0,0q�6�   $	��Top=�6�   $. 0.553,3.304&�6�   $
��Bold"�6�   $��3��6�   $��14(�6�   �� 5v     --   5        $/ txtLineWeight��5v     #��[ 65   $
��Left=�6i   $��30��6�   $��509,299,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��309��6�   $��Arial)�6�   $��3��6�   $��14.667(�6  {�.  $
��Bold"�6,  �� 5w     -.   5        $/ 
txtMCarton��5w     #��[ 62   $
��Left=�6f   $��25��6�   $��509,212,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��309��6�   $. 0.506,2.957&�6�   $��Arial)�6  $��3��6  $��16(�6)  {�.  $
��Bold"�6I  �� 5x     -/   5        $- lblVerCode_Copy2��5x     $
��QTY:��66   $��Arial)�6E   
�������$
��Left=�6k   $��672,257,0,0q�6�   $	��Top=�6�   $. 0.553,3.304&�6�   $��42��6�   $
��Bold"�6�   $��3��6�   $��14(�6  �� 5y     -0   5        $/ txtQty��5y     #��[ 6.   $
��Left=�6b   $��28��6}   $��717,256,0,0q�6�   $
0 Wrap=�6�   $�� ��6�   $	��Top=�6�   $	��101��6�   $. 0.506,2.957&�6�   $��Arial)�6  $��3��6  $��16(�6%  {�.  $
��Bold"�6E  �� 5z     -1   5        $/ txtError��5z     #��[ 60   $��Center=�6d   ; �� TextAlignment$; Center=�6�   $��68��6�   $��10,552,10,24q�6�   $
0 Wrap=�6�   $�� ��6�   $��Center=�6�   $
��1176��6�   
����������	#FFE60707$. 0.506,2.957&�6(  $��Arial)�6L  $��18(�6_  $
��Bold"�6m  $��4��6  {�.  6�  �� 5{     -2   5        $- label1_Copy��5{     $��User:��61   $
��Left=�6A   $��152,0,0,600q�6\   $��Bottom=�6q   $��Arial)�6�   $��16(�6�   ���	#FFF9F4F4$
��Bold"�6�   $��2��6�   $��2��6�   �� 5|     -3   5        $/ txtEmpNo��5|     #��[ 60   $
��Left=�6d   $��23��6   $��
115,17,0,0q�6�   $
0 Wrap=�6�   $	��Top=�6�   $	��199��6�   $��Arial)�6�   {�. $��3��6�   $��16(�6/  $
��Bold"�6=  6O  �� 5}     -4   5        $- lblSSID_Copy1��5}     $��MESSAGE:��63   $��Arial)�6F   
�������$
��Left=�6l   $��
10,522,0,0q�6�   $	��Top=�6�   $��18.667(�6�   $. 0.553,3.304&�6�   $	��134��6�   $��3��6�   $
��Bold"�6	  5~     �� 5     $��0,644,0,115q�6   $��Stretch=�61   
�������$	��0.6��6\   � 5�     -5   5        $# footer��5�     $��
0,726,0,-2q�6'   ���	#FF23536A 5�     �� 5�     $
��665* �6+   �� 5�     $
��531* �6+   .+�5�     �� -6   5        $( Status��5�      $��Center=�60   $��Center=�6M   $+ Center=�6h   ���	#FF23536A$��gfhfghg��6�   
�������$��Arial)�6�   $
��Bold"�6�   $��	10,4,29,7q�6�   $
��1157��6�   $��2��6�   5�     �� 5�     $��0,274,0,424q�6   $��Stretch=�61   
�������$	��0.6��6\   �� 5�     $��
0,723,0,25q�6   $��Stretch=�60   
�������$	��0.6��6[   5�     5�     5�     5�     