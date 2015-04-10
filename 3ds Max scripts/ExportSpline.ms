macroScript ExportSpline category:"ghiboz"
(
	for o in selection do
	( 
		nS = numSplines o
		print ("num total splines:"  +nS as string )
		
		nK = numKnots o
		print ("num total knots:"  + nK as string )
		
		din_ce = getsavefilename types:"xml|*.xml"
		if (din_ce != "") or (din_ce != undefined) do
		(
			fisier = createfile din_ce
			format "<xml>\n" to:fisier
			for s = 1 to (numSplines o) do
			(
				for k = 1 to (numKnots o s) do
				(
					knt = getKnotPoint o s k
					--format "\t<P pos=\"% % %\" />\n" (knt.x as string) (knt.y as string) (knt.z as string) to:fisier
					row = ("\t<P pos=\"" + knt.x as string + " " + knt.y as string + " " + knt.z as string + "\" />\n")
					print row
					format row to:fisier
				)--end k loop
			)--end s loop
			format "</xml>\n" to:fisier
			close fisier
		)
	)
)
