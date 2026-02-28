const TableSkeletonRows = ({ columns, rows = 3 }: { columns: number; rows?: number }) => {
    return (
        <>
            {Array.from({ length: rows }).map((_, rowIndex) => (
                <tr key={`skeleton-row-${rowIndex}`}>
                    {Array.from({ length: columns }).map((__, colIndex) => (
                        <td key={`skeleton-cell-${rowIndex}-${colIndex}`} className="px-4 py-3">
                            <div className="h-4 animate-pulse rounded bg-slate-200" />
                        </td>
                    ))}
                </tr>
            ))}
        </>
    );
};

export default TableSkeletonRows;
