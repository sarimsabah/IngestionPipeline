-- Run these queries in PostgreSQL to verify the customer data after posting

-- 1. Check the ingestion log (should show process_status = PROCESSED)
SELECT log_id, reference_id, status, http_status, process_status, processed_at
FROM log_customer_ingestion
WHERE raw_payload::jsonb->>'customerCode' = 'CRED9876'
ORDER BY received_at DESC LIMIT 1;

-- 2. Check master region table (should have UAE)
SELECT * FROM m_region WHERE region_code = 'UAE';

-- 3. Check master city table (should have Dubai)
SELECT * FROM m_city WHERE city_code = 'DXB';

-- 4. Check master payment term table (should have Net 30)
SELECT * FROM m_payment_term WHERE payment_term_code = 'Net 30';

-- 5. Check master channel table (should have Retail)
SELECT * FROM m_channel WHERE channel_code = 'Retail';

-- 6. Check transaction customer table (main customer record)
SELECT
    tc.customer_code,
    tc.customer_name,
    mr.region_name,
    mc.city_name,
    tc.address,
    tc.email_address,
    tc.customer_type,
    tc.credit_limit,
    mpt.payment_term_name,
    mch.channel_name,
    tc.is_active,
    tc.created_at
FROM t_customer tc
LEFT JOIN m_region mr ON tc.region_id = mr.id
LEFT JOIN m_city mc ON tc.city_id = mc.id
LEFT JOIN m_payment_term mpt ON tc.payment_term_id = mpt.id
LEFT JOIN m_channel mch ON tc.channel_id = mch.id
WHERE tc.customer_code = 'CRED9876';

-- 7. Check customer transactions (audit log - optional)
SELECT * FROM customer_transactions WHERE customer_code = 'CRED9876';
