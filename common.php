<?php

function collect_factors( array $a ) {
	$k = array_keys( $a );
	$i = array_fill_keys( range( 0, count($k) ), 0 );
	while( true ) {
		// パラメータ構築.
		$r = array();
		foreach ( $k as $x => $y ) {
			$r[$y] = $a[$y][ $i[$x] ];
		}
		yield $r;

		// 1要素だけずらす
		for ( $x=0; $x<count($k); ++$x ) {
			if ( ++$i[$x] < count( $a[ $k[$x] ] ) ) break;

			// $x番目の要素は終端まで走査したので次の要素を走査する。
			$i[$x] = 0;
		}
		if ( count($k) <= $x ) break; // 全ての要素を走査した。
	}
}

